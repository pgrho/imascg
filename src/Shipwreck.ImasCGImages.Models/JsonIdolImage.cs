using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Shipwreck.ImasCGImages.Models
{
    public class JsonIdolImage
    {
        /// <summary>
        /// <see cref="JsonIdolImage" />クラスの新しいインスタンスを初期化します。
        /// </summary>
        public JsonIdolImage()
        {
        }

        public JsonIdolImage(IdolImage model)
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

        public string GetUrl(IdolImageDataType type)
        {
            switch (type)
            {
                case IdolImageDataType.Framed:
                    return ImageUrl;
                case IdolImageDataType.Frameless:
                    return FramelessImageUrl;
                case IdolImageDataType.Quest:
                    return QuestImageUrl;
                case IdolImageDataType.LS:
                    return BannerImageUrl;
                case IdolImageDataType.XS:
                    return IconImageUrl;
            }
            return null;
        }

        public void SetUrl(IdolImageDataType type, string value)
        {
            switch (type)
            {
                case IdolImageDataType.Framed:
                    ImageUrl = value;
                    break;

                case IdolImageDataType.Frameless:
                    FramelessImageUrl = value;
                    break;

                case IdolImageDataType.Quest:
                    QuestImageUrl = value;
                    break;

                case IdolImageDataType.LS:
                    BannerImageUrl = value;
                    break;

                case IdolImageDataType.XS:
                    IconImageUrl = value;
                    break;
            }
        }

        #endregion Url
    }
}