using Shipwreck.ImasCGImages.Models;
using Shipwreck.ImasCGImages.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Shipwreck.ImasCGImages.WebApi.Controllers
{
    public class ImageController : Controller
    {
        public ActionResult Index()
            => View();

        public async Task<ActionResult> Search(
            string hash = null,
            string headline = null, TextOperator headlineOperator = TextOperator.Contains,
            string name = null, TextOperator nameOperator = TextOperator.Equal,
            string kana = null, TextOperator kanaOperator = TextOperator.Equal,
            IdolRarity rarity = IdolRarity.Unknown, bool? isPlus = null,
            IdolType idolType = IdolType.Unknown,
            int count = 32)
        {
            using (var db = new ImasCDDbContext())
            {
                IQueryable<IdolImage> q = db.IdolImages;

                if (!string.IsNullOrEmpty(hash))
                {
                    q = q.Where(_ => _.Hash == hash);
                }

                if (!string.IsNullOrEmpty(headline))
                {
                    switch (headlineOperator)
                    {
                        case TextOperator.Equal:
                            q = q.Where(_ => _.Headline == headline);
                            break;

                        case TextOperator.Contains:
                            q = q.Where(_ => _.Headline.Contains(headline));
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(name))
                {
                    switch (nameOperator)
                    {
                        case TextOperator.Equal:
                            q = q.Where(_ => _.Idol.Name == name);
                            break;

                        case TextOperator.Contains:
                            q = q.Where(_ => _.Idol.Name.Contains(name));
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(kana))
                {
                    switch (kanaOperator)
                    {
                        case TextOperator.Equal:
                            q = q.Where(_ => _.Idol.Kana == kana);
                            break;

                        case TextOperator.Contains:
                            q = q.Where(_ => _.Idol.Kana.Contains(kana));
                            break;
                    }
                }
                if (isPlus == true || (rarity & IdolRarity.Plus) == IdolRarity.Plus)
                {
                    q = q.Where(_ => (_.Rarity & IdolRarity.Plus) == IdolRarity.Plus);
                }
                else if (isPlus == false)
                {
                    q = q.Where(_ => (_.Rarity & IdolRarity.Plus) != IdolRarity.Plus);
                }

                rarity &= ~IdolRarity.Plus;
                if (rarity != IdolRarity.Unknown)
                {
                    q = q.Where(_ => (_.Rarity & rarity) == rarity);
                }
                if (idolType != IdolType.Unknown)
                {
                    q = q.Where(_ => _.Type == idolType);
                }
                var list = await q.Include(_ => _.Idol).Take(Math.Max(count, 1)).ToListAsync();

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

                var ub = new UriBuilder(Request.Url);
                ub.Query = null;

                foreach (var i in list)
                {
                    var j = new JsonIdolImage(i);
                    foreach (var t in types)
                    {
                        bool b;
                        if (!ie.TryGetValue(new { j.Hash, Type = t }, out b) || b)
                        {
                            ub.Path = Url.Action(nameof(Image), "Image", new { hash = j.Hash, type = t });
                            j.SetUrl(t, ub.Uri.ToString());
                        }
                    }
                    jl.Add(j);
                }

                return Json(new JsonIdolImageResult()
                {
                    Items = jl
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<ActionResult> Image(string hash, IdolImageDataType type)
        {
            using (var db = new ImasCDDbContext())
            {
                var d = await db.IdolImageData.FirstOrDefaultAsync(_ => _.Hash == hash && _.Type == type);

                if (d != null)
                {
                    if (d.Data == null)
                    {
                        return HttpNotFound();
                    }
                    return File(d.Data, "image/jpeg");
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
                    return HttpNotFound();
                }
                return File(data, "image/jpeg");
            }
        }
    }
}