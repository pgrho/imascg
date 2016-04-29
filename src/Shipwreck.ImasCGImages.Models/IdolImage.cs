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
    public class IdolImage
    {
        public int IdolId { get; set; }

        [ForeignKey(nameof(IdolId))]
        public virtual Idol Idol { get; set; }

        [Key]
        [StringLength(64)]
        public string Hash { get; set; }

        [StringLength(255)]
        public string Headline { get; set; }

        public IdolRarity Rarity { get; set; }

        public IdolType Type { get; set; }

        public BloodType BloodType { get; set; }

        public float Height { get; set; }

        [StringLength(32)]
        public string SpecialHeight { get; set; }

        public float Weight { get; set; }

        [StringLength(32)]
        public string SpecialWeight { get; set; }

        public float Bust { get; set; }
        public float Waist { get; set; }
        public float Hip { get; set; }


        [StringLength(32)]
        public string SpecialBust { get; set; }


        [StringLength(32)]
        public string SpecialWaist { get; set; }


        [StringLength(32)]
        public string SpecialHip { get; set; }

        public float Age { get; set; }


        [StringLength(32)]
        public string SpecialAge { get; set; }

        public byte BirthMonth { get; set; }

        public byte BirthDay { get; set; }

        [StringLength(32)]
        public string SpecialBirthday { get; set; }

        public SunSign SunSign { get; set; }

        [StringLength(32)]
        public string SpecialSunSign{ get; set; }

        [StringLength(64)]
        public string Birthplace { get; set; }

        [StringLength(64)]
        public string Hobby { get; set; }

        public Handedness Handedness { get; set; }

        //public string ImageUrl
        //    => $"http://gamedb.squares.net/idolmaster/image_sp/card/l/{Hash}.jpg";

        //public string NoFrameImageUrl
        //    => $"http://gamedb.squares.net/idolmaster/image_sp/card/l_noframe/{Hash}.jpg";

        //public string QuestImageUrl
        //    => $"http://gamedb.squares.net/idolmaster/image_sp/card/quest/{Hash}.jpg";

        //public string LSImageUrl
        //    => $"http://gamedb.squares.net/idolmaster/image_sp/card/ls/{Hash}.jpg";

        //public string XSImageUrl
        //    => $"http://gamedb.squares.net/idolmaster/image_sp/card/xs/{Hash}.jpg";
    }
}