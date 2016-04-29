using HtmlAgilityPack;
using Shipwreck.ImasCGImages.Models.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Shipwreck.ImasCGImages.Models
{
    public class Idol
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [StringLength(64)]
        public string Name { get; set; }

        [StringLength(64)]
        public string Kana { get; set; }

        public static async Task<List<Idol>> DownloadIdolsAsync(string listUrl = null)
        {
            var doc = new HtmlDocument();
            using (var wc = new WebClient())
            {
                var data = await wc.DownloadDataTaskAsync(listUrl ?? Settings.Default.IdolListUrl);

                using (var ms = new MemoryStream(data))
                using (var sr = new StreamReader(ms, Encoding.UTF8))
                {
                    doc.Load(sr);
                }
            }

            var idols = new List<Idol>();
            foreach (var anchor in doc.DocumentNode.Descendants("a"))
            {
                if (!anchor.GetAttributeValue("class", "").Contains("idol-link"))
                {
                    continue;
                }

                var href = anchor.GetAttributeValue("href", string.Empty);

                var li = anchor.Ancestors("li").First();

                var nameValue = li.GetAttributeValue("n", string.Empty);

                var m = Regex.Match(nameValue, @"^(\[[^\]]+\])?(?<n>[^\s]+)\s*(?<k>.+)$");

                int id;
                var ng = m.Groups["n"];
                var kg = m.Groups["k"];

                if (int.TryParse(href.Split('=').Last(), out id) && ng.Success && kg.Success)
                {
                    idols.Add(new Idol()
                    {
                        Id = id,
                        Name = ng.Value,
                        Kana = kg.Value
                    });
                }
            }

            return idols;
        }

        public async Task<List<IdolImage>> GetIdolImagesAsync(string urlFormat = null, Action<string, string> errorHandler = null)
        {
            var doc = new HtmlDocument();

            using (var wc = new WebClient())
            {
                var data = await wc.DownloadDataTaskAsync(string.Format(urlFormat ?? Settings.Default.IdolImageUrlFormat, +Id));

                using (var ms = new MemoryStream(data))
                using (var sr = new StreamReader(ms, Encoding.UTF8))
                {
                    doc.Load(sr);
                }
            }

            var imgs = new List<IdolImage>();
            foreach (var a in doc.DocumentNode.Descendants("a").Where(a => a.GetAttributeValue("class", "") == "swap-card"))
            {
                var div = a.ParentNode.ParentNode;

                var inftable = div.Descendants("table").Where(_ => _.GetAttributeValue("class", "") == "bcinf").FirstOrDefault(_ => _.Descendants("th").Any(h => h.InnerText.Contains("レア度")));

                var tr1 = inftable.Descendants("tr").ElementAt(1).Descendants("td").ToArray();
                var tr3 = inftable.Descendants("tr").ElementAt(3).Descendants("td").ToArray();

                var img = new IdolImage()
                {
                    Idol = this,
                    IdolId = Id,
                    Headline = div.Descendants("h2").FirstOrDefault(_ => _.GetAttributeValue("class", "") == "headline")?.InnerText?.Trim(),
                    Hash = a.GetAttributeValue("href", "").Split('=').Last(),
                };

                img.Rarity = GetRarity(tr1[0].InnerText, img.Headline, errorHandler);
                img.Type = GetType(tr1[1].InnerText, errorHandler);
                img.BloodType = GetBloodType(tr1[2].InnerText, errorHandler);

                SetHeight(img, tr1[3].InnerText?.Trim(), errorHandler);
                SetWeight(img, tr1[4].InnerText?.Trim(), errorHandler);
                SetThreeSize(img, tr1[5].InnerText?.Trim(), errorHandler);

                SetAge(img, tr3[0].InnerText?.Trim(), errorHandler);
                SetBirthday(img, tr3[1].InnerText?.Trim(), errorHandler);
                img.SunSign = GetSunSign(tr3[2].InnerText, errorHandler);
                img.Birthplace = tr3[3].InnerText.Trim();
                img.Hobby = tr3[4].InnerText.Trim();
                img.Handedness = GetHandedness(tr3[5].InnerText, errorHandler);

                imgs.Add(img);
            }

            return imgs;
        }

        #region IdolImage解析

        private IdolRarity GetRarity(string value, string headline, Action<string, string> errorHandler)
        {
            var v = (value ?? string.Empty).Trim().ToLowerInvariant();
            var rt = IdolRarity.Unknown;
            switch (v)
            {
                case "normal":
                    rt = IdolRarity.Normal;
                    break;

                case "rare":
                    rt = IdolRarity.Rare;
                    break;

                case "srare":
                    rt = IdolRarity.SRare;
                    break;

                default:

                    errorHandler?.Invoke(nameof(IdolImage.Rarity), value);

                    break;
            }
            if (rt != IdolRarity.Unknown && headline?.EndsWith("+") == true)
            {
                rt |= IdolRarity.Plus;
            }
            return rt;
        }

        private IdolType GetType(string value, Action<string, string> errorHandler)
        {
            var v = (value ?? string.Empty).Trim().ToLowerInvariant();
            switch (v)
            {
                case "cute":
                    return IdolType.Cute;

                case "cool":
                    return IdolType.Cool;

                case "passion":
                    return IdolType.Passion;

                case "trainer":
                    return IdolType.Trainer;

                default:

                    errorHandler?.Invoke(nameof(IdolImage.Type), value);
                    return IdolType.Unknown;
            }
        }

        private BloodType GetBloodType(string value, Action<string, string> errorHandler)
        {
            var v = (value ?? string.Empty).Trim().TrimEnd('型').ToLowerInvariant();
            switch (v)
            {
                case "a":
                    return BloodType.A;

                case "b":
                    return BloodType.B;

                case "o":
                    return BloodType.O;

                case "ab":
                    return BloodType.AB;

                default:
                    errorHandler?.Invoke(nameof(IdolImage.BloodType), value);
                    return BloodType.Unknown;
            }
        }

        private HashSet<string> _UnknownNumber;

        private float GetNumber(string value, Action<string, string> errorHandler)
        {
            var v = (value ?? string.Empty).Trim().ToLowerInvariant().TrimEnd('k', 'g', 'c', 'm', '才', '歳');
            float f;
            if (float.TryParse(v, out f))
            {
                return f;
            }

            errorHandler?.Invoke(nameof(IdolImage.BloodType), value);

            return 0;
        }

        private void SetHeight(IdolImage img, string heightString, Action<string, string> errorHandler)
        {
            img.Height = GetNumber(heightString, errorHandler);

            if (img.Height == 0 && heightString?.Length > 0)
            {
                img.SpecialHeight = heightString;
            }
        }

        private void SetWeight(IdolImage img, string weightString, Action<string, string> errorHandler)
        {
            img.Weight = GetNumber(weightString, errorHandler);

            if (img.Weight == 0 && weightString?.Length > 0)
            {
                img.SpecialWeight = weightString;
            }
        }

        private void SetThreeSize(IdolImage image, string value, Action<string, string> errorHandler)
        {
            var m = Regex.Match(value ?? string.Empty, @"^\s*(?<b>[0-9]{1,3})\s*/\s*(?<w>[0-9]{1,3})\s*/\s*(?<h>[0-9]{1,3})\s*$");

            if (m.Success)
            {
                byte b, w, h;

                if (byte.TryParse(m.Groups["b"].Value, out b)
                    && byte.TryParse(m.Groups["w"].Value, out w)
                    && byte.TryParse(m.Groups["h"].Value, out h))
                {
                    image.Bust = b;
                    image.Waist = w;
                    image.Hip = h;
                    return;
                }
            }

            if (!string.IsNullOrEmpty(value))
            {
                var vs = value.Split('/');

                if (vs.Length == 3)
                {
                    image.SpecialBust = vs[0].Trim();
                    image.SpecialWaist = vs[1].Trim();
                    image.SpecialHip = vs[2].Trim();
                }
                else
                {
                    image.SpecialBust = value;
                    image.SpecialWaist = null;
                    image.SpecialHip = null;
                }

                errorHandler?.Invoke(nameof(image.Bust), value);
            }
        }

        private void SetAge(IdolImage img, string ageString, Action<string, string> errorHandler)
        {
            img.Age = GetNumber(ageString, errorHandler);

            if (img.Age == 0 && ageString?.Length > 0)
            {
                img.SpecialAge = ageString;
            }
        }

        private void SetBirthday(IdolImage image, string value, Action<string, string> errorHandler)
        {
            var m = Regex.Match(value ?? string.Empty, @"^\s*(?<m>[0-9]{1,2})\s*月\s*(?<d>[0-9]{1,2})\s*日\s*$");

            if (m.Success)
            {
                byte b, w;

                if (byte.TryParse(m.Groups["m"].Value, out b)
                    && byte.TryParse(m.Groups["d"].Value, out w))
                {
                    image.BirthDay = b;
                    image.BirthMonth = w;
                    return;
                }
            }

            if (!string.IsNullOrEmpty(value))
            {
                image.SpecialBirthday = value;
                errorHandler?.Invoke(nameof(image.BirthMonth), value);
            }
        }

        private SunSign GetSunSign(string value, Action<string, string> errorHandler)
        {
            var v = (value ?? string.Empty).Trim().ToLowerInvariant();
            switch (v)
            {
                case "山羊座":
                    return SunSign.Capricorn;

                case "水瓶座":
                    return SunSign.Aquarian;

                case "魚座":
                    return SunSign.Piscean;

                case "牡羊座":
                    return SunSign.Arian;

                case "牡牛座":
                    return SunSign.Taurean;

                case "双子座":
                    return SunSign.Geminian;

                case "蟹座":
                case "かに座":
                    return SunSign.Cancerian;

                case "獅子座":
                    return SunSign.Leo;

                case "乙女座":
                    return SunSign.Virgin;

                case "天秤座":
                    return SunSign.Libran;

                case "蠍座":
                    return SunSign.Scorpio;

                case "射手座":
                    return SunSign.Sagittarian;

                default:
                    errorHandler?.Invoke(nameof(IdolImage.SunSign), value);
                    return SunSign.Unknown;
            }
        }

        private Handedness GetHandedness(string value, Action<string, string> errorHandler)
        {
            var v = (value ?? string.Empty).Trim().ToLowerInvariant();
            switch (v)
            {
                case "右":
                    return Handedness.Right;

                case "左":
                    return Handedness.Left;

                case "両":
                    return Handedness.Both;

                default:
                    errorHandler?.Invoke(nameof(IdolImage.Handedness), value);
                    return Handedness.Unknown;
            }
        }

        #endregion IdolImage解析
    }
}