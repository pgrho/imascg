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
    [Flags]
    public enum IdolRarity : byte
    {
        Unknown = 0,

        Normal = 1,
        Rare = 2,
        SRare = 4,
        Plus = 128,

        NormalPlus = Normal | Plus,
        RarePlus = Rare | Plus,
        SRarePlus = SRare | Plus,
    }
}