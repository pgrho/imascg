using Shipwreck.ImasCGImages.Models;
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
        public async Task<ActionResult> Index(string name = null, NameOperator nameOperator = NameOperator.Equal, string kana = null, NameOperator kanaOperator = NameOperator.Equal, IdolRarity rarity = IdolRarity.Unknown, bool? isPlus = null)
        {
            using (var db = new ImasCDDbContext())
            {
                IQueryable<IdolImage> q = db.IdolImages;
                if (!string.IsNullOrEmpty(name))
                {
                    switch (nameOperator)
                    {
                        case NameOperator.Equal:
                            q = q.Where(_ => _.Idol.Name == name);
                            break;

                        case NameOperator.Contains:
                            q = q.Where(_ => _.Idol.Name.Contains(name));
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(kana))
                {
                    switch (kanaOperator)
                    {
                        case NameOperator.Equal:
                            q = q.Where(_ => _.Idol.Kana == kana);
                            break;

                        case NameOperator.Contains:
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
                var list = await q.Include(_ => _.Idol).Take(32).ToListAsync();

                var jl = new List<JsonIdolImage>(list.Count);

                var ub = new UriBuilder(Request.Url);
                ub.Query = null;

                foreach (var i in list)
                {
                    var j = new JsonIdolImage(i);

                    ub.Path = Url.Action(nameof(Image), "Image", new { hash = i.Hash, type = IdolImageDataType.Framed });
                    j.FrameUrl = ub.Uri.ToString();

                    ub.Path = Url.Action(nameof(Image), "Image", new { hash = i.Hash, type = IdolImageDataType.Frameless });
                    j.FramelessUrl = ub.Uri.ToString();

                    ub.Path = Url.Action(nameof(Image), "Image", new { hash = i.Hash, type = IdolImageDataType.Quest });
                    j.QuestUrl = ub.Uri.ToString();

                    ub.Path = Url.Action(nameof(Image), "Image", new { hash = i.Hash, type = IdolImageDataType.LS });
                    j.LSUrl = ub.Uri.ToString();

                    ub.Path = Url.Action(nameof(Image), "Image", new { hash = i.Hash, type = IdolImageDataType.XS });
                    j.XSUrl = ub.Uri.ToString();

                    jl.Add(j);
                }

                return Json(jl, JsonRequestBehavior.AllowGet);
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