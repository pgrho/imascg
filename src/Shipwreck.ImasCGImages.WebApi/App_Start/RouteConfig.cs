using System;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Shipwreck.ImasCGImages.WebApi
{

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "ImageUrl",
                url: "i/{type}/{hash}.jpg",
                defaults: new { controller = "Image", action = "Image" }
            );
            //routes.MapRoute(
            //    name: "FishOriginalImage",
            //    url: "Fish/OriginalImage/{id}.jpg",
            //    defaults: new { controller = "Fish", action = "OriginalImage", id = UrlParameter.Optional }
            //);

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Image", action = "Index", id = UrlParameter.Optional }
            );
        }
    }

}