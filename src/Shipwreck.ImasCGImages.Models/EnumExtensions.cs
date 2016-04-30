using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.ImasCGImages.Models
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this SunSign value)
        {
            switch (value)
            {
                case SunSign.Capricorn:
                    return "山羊座";

                case SunSign.Aquarian:
                    return "水瓶座";

                case SunSign.Piscean:
                    return "魚座";

                case SunSign.Arian:
                    return "牡羊座";

                case SunSign.Taurean:
                    return "牡牛座";

                case SunSign.Geminian:
                    return "双子座";

                case SunSign.Cancerian:
                    return "蟹座";

                case SunSign.Leo:
                    return "獅子座";

                case SunSign.Virgin:
                    return "乙女座";

                case SunSign.Libran:
                    return "天秤座";

                case SunSign.Scorpio:
                    return "蠍座";

                case SunSign.Sagittarian:
                    return "射手座";

                default:
                    return null;
            }
        }
    }
}
