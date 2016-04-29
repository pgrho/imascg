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
    public enum IdolImageDataType : byte
    {
        Framed = 0,
        Frameless = 1,
        Quest = 2,
        LS = 3,
        XS = 4
    }
}