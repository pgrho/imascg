using Shipwreck.ImasCGImages.Models;
using Shipwreck.ImasCGImages.WebApi.Controllers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Shipwreck.ImasCGImages.WebApi.Models
{
    public class JsonIdolImage
    {
        /// <summary>
        /// <see cref="JsonIdolImage" />クラスの新しいインスタンスを初期化します。
        /// </summary>
        public JsonIdolImage()
        {
        }

        internal JsonIdolImage(IdolImage model, Controller controller)
        {
            Headline = model.Headline;
            Name = model.Idol?.Name;
            Kana = model.Idol?.Kana;
            Hash = model.Hash;
            Rarity = model.Rarity.ToString();
            IdolType = model.Type.ToString();

            BloodType = model.BloodType.ToString();
            Height = model.SpecialHeight ?? $"{model.Height}cm";
            Weight = model.SpecialWeight ?? $"{model.Weight}kg";
            Bust = model.SpecialBust ?? $"{model.Bust}";
            Waist = model.SpecialWaist ?? $"{model.Waist}";
            Hip = model.SpecialHip ?? $"{model.Hip}";
            Age = model.SpecialAge ?? $"{model.Age}歳";
            Birthday = model.SpecialBirthday ?? $"{model.BirthDay}月{model.BirthMonth}日";
            SunSign = model.SpecialSunSign ?? model.SunSign.GetDisplayName();
            Birthplace = model.Birthplace;
            Hobby = model.Hobby;
            Handedness = model.Handedness.ToString();

            ImageUrl = ImageController.GetImageUri(controller, Hash, IdolImageDataType.Framed).ToString();
            FramelessImageUrl = ImageController.GetImageUri(controller, Hash, IdolImageDataType.Frameless).ToString();
            QuestImageUrl = ImageController.GetImageUri(controller, Hash, IdolImageDataType.Quest).ToString();
            BannerImageUrl = ImageController.GetImageUri(controller, Hash, IdolImageDataType.LS).ToString();
            IconImageUrl = ImageController.GetImageUri(controller, Hash, IdolImageDataType.XS).ToString();
        }

        public string Headline { get; set; }

        public string Name { get; set; }

        public string Kana { get; set; }

        public string Hash { get; set; }

        public string Rarity { get; set; }

        public string IdolType { get; set; }

        public string BloodType { get; set; }

        public string Height { get; set; }
        public string Weight { get; set; }
        public string Bust { get; set; }
        public string Waist { get; set; }
        public string Hip { get; set; }
        public string Age { get; set; }
        public string Birthday { get; set; }
        public string SunSign { get; set; }
        public string Birthplace { get; set; }

        public string Hobby { get; set; }

        public string Handedness { get; set; }

        #region Url

        public string ImageUrl { get; set; }
        public string FramelessImageUrl { get; set; }
        public string QuestImageUrl { get; set; }
        public string BannerImageUrl { get; set; }
        public string IconImageUrl { get; set; }

        #endregion Url
    }
}