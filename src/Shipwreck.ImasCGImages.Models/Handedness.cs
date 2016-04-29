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
    public enum Handedness : byte
    {
        Unknown = 0,
        Right = 1,
        Left = 2,
        Both = 3
    }
}