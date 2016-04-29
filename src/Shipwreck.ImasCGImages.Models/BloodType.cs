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
    public enum BloodType : byte
    {
        Unknown = 0,
        A = 1,
        B = 2,
        O = 3,
        AB = 4,
    }
}