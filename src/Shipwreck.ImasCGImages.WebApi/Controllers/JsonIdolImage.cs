using Shipwreck.ImasCGImages.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Shipwreck.ImasCGImages.WebApi.Controllers
{
    public class JsonIdolImage
    {
        public JsonIdolImage()
        {
        }

        public JsonIdolImage(IdolImage model)
        {
            Headline = model.Headline;
            Name = model.Idol?.Name;
            Kana = model.Idol?.Kana;
            Hash = model.Hash;
            Rarity = model.Rarity;
            IdolType = model.Type;
        }

        public string Headline { get; set; }

        public string Name { get; set; }

        public string Kana { get; set; }

        public string Hash { get; set; }

        public IdolRarity Rarity { get; set; }

        public IdolType IdolType { get; set; }

        public string FrameUrl { get; set; }
        public string FramelessUrl { get; set; }
        public string QuestUrl { get; set; }
        public string LSUrl { get; set; }
        public string XSUrl { get; set; }
    }
}