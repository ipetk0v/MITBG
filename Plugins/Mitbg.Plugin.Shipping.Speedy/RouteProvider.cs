using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Shipping.Speedy
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapRoute("Admin.Speedy.Configure", "Admin/Speedy/Configure",
                new { controller = "Speedy", action = "Configure" });

            routeBuilder.MapRoute("Plugin.Shipping.Speedy.SitesHandler", "Plugins/SpeedyShipping/GetSites",
                new { controller = "Speedy", action = "GetSites" });

            routeBuilder.MapRoute("Plugin.Shipping.Speedy.OfficiesHandler", "Plugins/SpeedyShipping/GetOfficies",
                new { controller = "Speedy", action = "GetOfficies" });

            routeBuilder.MapRoute("Plugin.Shipping.Speedy.ListStreets", "Plugins/SpeedyShipping/GetStreets",
                new { controller = "Speedy", action = "GetStreets" });

            routeBuilder.MapRoute("Plugin.Shipping.Speedy.ListQuarters", "Plugins/SpeedyShipping/GetQuarters",
                new { controller = "Speedy", action = "GetQuarters" });

            routeBuilder.MapRoute("Plugin.Shipping.Speedy.FormHandler", "Plugins/SpeedyShipping/SpeedyForm",
                new { controller = "Speedy", action = "SpeedyForm" });

            routeBuilder.MapRoute("Plugin.Shipping.Speedy.CreatePdf", "Plugins/SpeedyShipping/CreatePdf",
                new { controller = "Speedy", action = "CreatePdf" });

            routeBuilder.MapRoute("Plugin.Shipping.Speedy.SaveForm", "Plugins/SpeedyShipping/SaveForm",
                new { controller = "Speedy", action = "SaveForm" });

            routeBuilder.MapRoute("Plugin.Shipping.Speedy.CancelShipping", "Plugins/SpeedyShipping/CancelShipping",
                         new { controller = "Speedy", action = "CancelShipping" });

            routeBuilder.MapRoute("Plugin.Shipping.Speedy.CourierPicking", "Plugins/SpeedyShipping/CourierPicking",
                                  new { controller = "Speedy", action = "CourierPicking" });

            routeBuilder.MapRoute("Plugin.Shipping.Speedy.ShipmentsList", "Plugins/SpeedyShipping/ShipmentsList",
                new { controller = "Speedy", action = "ShipmentsList" });


        }
        public int Priority
        {
            get
            {
                return -1;
            }
        }
    }
}
