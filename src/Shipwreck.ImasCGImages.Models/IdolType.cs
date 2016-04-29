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
    public enum IdolType : byte
    {
        Unknown = 0,
        Cute = 1,
        Cool = 2,
        Passion = 3,
        Trainer = 4
    }
}