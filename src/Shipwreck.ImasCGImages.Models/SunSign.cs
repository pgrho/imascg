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
    public enum SunSign : byte
    {
        Unknown = 0,
        Capricorn = 1,
        Aquarian = 2,
        Piscean = 3,
        Arian = 4,
        Taurean = 5,
        Geminian = 6,
        Cancerian = 7,
        Leo = 8,
        Virgin = 9,
        Libran = 10,
        Scorpio = 11,
        Sagittarian = 12,
    }
}