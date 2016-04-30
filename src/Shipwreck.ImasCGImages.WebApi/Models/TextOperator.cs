using Shipwreck.ImasCGImages.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Shipwreck.ImasCGImages.WebApi.Models
{
    public enum TextOperator
    {
        Equal = 0,
        Contains = 1,
    }
}