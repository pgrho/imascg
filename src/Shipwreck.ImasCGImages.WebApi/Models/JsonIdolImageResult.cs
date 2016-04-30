using Shipwreck.ImasCGImages.Models;
using Shipwreck.ImasCGImages.WebApi.Controllers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Shipwreck.ImasCGImages.WebApi.Models
{
    public class JsonIdolImageResult
    {
        public List<JsonIdolImage> Items { get; set; }
    }
}