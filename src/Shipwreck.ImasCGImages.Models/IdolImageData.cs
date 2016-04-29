using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.ImasCGImages.Models
{
    public class IdolImageData
    {
        [Key]
        [Column(Order = 0)]
        public string Hash { get; set; }

        [Key]
        [Column(Order = 1)]
        public IdolImageDataType Type { get; set; }

        public byte[] Data { get; set; }

        [ForeignKey(nameof(Hash))]
        public virtual IdolImage Image { get; set; }

        public static string GetImageUrl(string hash, IdolImageDataType type)
        {
            string s;
            switch (type)
            {
                case IdolImageDataType.Framed:
                    s = "l";
                    break;

                case IdolImageDataType.Frameless:
                    s = "l_noframe";
                    break;

                case IdolImageDataType.Quest:
                    s = "quest";
                    break;

                case IdolImageDataType.LS:
                    s = "ls";
                    break;

                case IdolImageDataType.XS:
                    s = "xs";
                    break;

                default:
                    return null;
            }

            return $"http://gamedb.squares.net/idolmaster/image_sp/card/{s}/{hash}.jpg";
        }
    }
}