using Shipwreck.ImasCGImages.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Shipwreck.ImasCGImages.WebApi.Controllers
{
    public enum NameOperator
    {
        Equal = 0,
        Contains = 1,
    }
}