using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Mitbg.Plugin.Misc.XmlAutomationImport
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            //routeBuilder.MapRoute("Plugin.XmlAutomationImport.TemplateList", "/Plugins/XmlAutomationImport/TemplateList",
            //    new { controller = "XmlAutomationImport", action = "TemplateList" });

            routeBuilder.MapRoute("Plugin.XmlAutomationImport.TemplateList", "Admin/XmlAutomationImport/TemplateList/",
                new { controller = "XmlAutomationImport", action = "TemplateList" });

            routeBuilder.MapRoute("Plugin.XmlAutomationImport.CreateTemplate", "Admin/XmlAutomationImport/CreateTemplate/",
                new { controller = "XmlAutomationImport", action = "CreateTemplate" });

            routeBuilder.MapRoute("Plugin.XmlAutomationImport.EditTemplate", "Admin/XmlAutomationImport/EditTemplate/",
                new { controller = "XmlAutomationImport", action = "EditTemplate" });
        }

        public int Priority => 0;
    }
}
