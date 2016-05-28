using Shipwreck.ImasCGImages.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Shipwreck.ImasCGImages.WebApi.Controllers
{
    public class ImagesController : ApiController
    {
        [Route("Image/Search")]
        [Route("api/search")]
        public async Task<JsonIdolImageResult> GetItemsAsync([FromUri]SearchCondition condition = null)
        {
            var c = condition ?? new SearchCondition();
            using (var db = new ImasCDDbContext())
            {
                IQueryable<IdolImage> q = db.IdolImages;
                q = Where(q, _ => _.Hash, c.Hash, TextOperator.Equal);
                q = Where(q, _ => _.Headline, c.Headline, c.HeadlineOperator);
                q = Where(q, _ => _.Idol.Name, c.Name, c.NameOperator);
                q = Where(q, _ => _.Idol.Kana, c.Kana, c.KanaOperator);

                if (c.IsPlus == true || (c.Rarity & IdolRarity.Plus) == IdolRarity.Plus)
                {
                    q = q.Where(_ => (_.Rarity & IdolRarity.Plus) == IdolRarity.Plus);
                }
                else if (c.IsPlus == false)
                {
                    q = q.Where(_ => (_.Rarity & IdolRarity.Plus) != IdolRarity.Plus);
                }

                c.Rarity &= ~IdolRarity.Plus;
                if (c.Rarity != IdolRarity.Unknown)
                {
                    q = q.Where(_ => (_.Rarity & c.Rarity) == c.Rarity);
                }
                if (c.IdolType != IdolType.Unknown)
                {
                    q = q.Where(_ => _.Type == c.IdolType);
                }
                var list = await q.Include(_ => _.Idol).Take(Math.Max(c.Count, 1)).ToListAsync();

                var types = new[]
                {
                    IdolImageDataType.Framed,
                    IdolImageDataType.Frameless,
                    IdolImageDataType.Quest,
                    IdolImageDataType.LS,
                    IdolImageDataType.XS
                };

                var hs = list.Select(_ => _.Hash);
                var ie = await db.IdolImageData.Where(_ => hs.Contains(_.Hash) && types.Contains(_.Type)).ToDictionaryAsync(_ => new { _.Hash, _.Type }, _ => _.Data != null);

                var jl = new List<JsonIdolImage>(list.Count);

                IEnumerable<string> vs;
                Request.Headers.TryGetValues("X-Original-URL", out vs);

                var ub = new UriBuilder(vs?.FirstOrDefault() ?? Request.RequestUri.ToString());
                ub.Query = null;

                foreach (var i in list)
                {
                    var j = new JsonIdolImage(i);
                    foreach (var t in types)
                    {
                        bool b;
                        if (!ie.TryGetValue(new { j.Hash, Type = t }, out b) || b)
                        {
                            ub.Path = $"{RequestContext.VirtualPathRoot}i/{t}/{j.Hash}.jpg";
                            j.SetUrl(t, ub.Uri.ToString());
                        }
                    }
                    jl.Add(j);
                }

                return new JsonIdolImageResult()
                {
                    Items = jl
                };
            }
        }

        private static IQueryable<T> Where<T>(IQueryable<T> source, Expression<Func<T, string>> selector, string value, TextOperator @operator)
        {
            if (string.IsNullOrEmpty(value))
            {
                return source;
            }
            switch (@operator)
            {
                case TextOperator.Equal:
                    return source.Where(Expression.Lambda<Func<T, bool>>(Expression.Equal(selector.Body, Expression.Constant(value)), selector.Parameters[0]));

                case TextOperator.Contains:
                    return source.Where(
                            Expression.Lambda<Func<T, bool>>(
                                Expression.Call(
                                    selector.Body,
                                    nameof(string.Contains),
                                    null,
                                    Expression.Constant(value)),
                                selector.Parameters[0]));

                default:
                    throw new NotSupportedException();
            }
        }

        [Route("i/{type}/{hash}.jpg")]
        public async Task<HttpResponseMessage> GetImageAsync(string hash, IdolImageDataType type)
        {
            using (var db = new ImasCDDbContext())
            {
                var d = await db.IdolImageData.FirstOrDefaultAsync(_ => _.Hash == hash && _.Type == type);

                if (d != null)
                {
                    if (d.Data == null)
                    {
                        throw new HttpResponseException(HttpStatusCode.NotFound);
                    }
                    return CreateFileResult(d.Data, "image/jpeg");
                }

                var u = IdolImageData.GetImageUrl(hash, type);

                byte[] data;
                using (var wc = new WebClient())
                {
                    try
                    {
                        data = await wc.DownloadDataTaskAsync(u);
                    }
                    catch
                    {
                        data = null;
                    }
                }

                db.IdolImageData.Add(new IdolImageData()
                {
                    Hash = hash,
                    Type = type,
                    Data = data
                });

                await db.SaveChangesAsync();

                if (data == null)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }
                return CreateFileResult(data, "image/jpeg");
            }
        }

        private HttpResponseMessage CreateFileResult(byte[] data, string mimeType)
        {
            var r = new HttpResponseMessage(HttpStatusCode.OK);
            r.Content = new ByteArrayContent(data);
            r.Content.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
            return r;
        }
    }
}