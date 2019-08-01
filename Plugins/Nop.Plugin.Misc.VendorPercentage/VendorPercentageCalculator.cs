using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Domain.Tasks;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.Misc.VendorPercentage
{
    public partial class VendorPercentageCalculator : BasePlugin, IAdminMenuPlugin
    {

        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;

        public VendorPercentageCalculator(ILocalizationService localizationService, IPermissionService permissionService, IWorkContext workContext)
        {
            _localizationService = localizationService;
            _permissionService = permissionService;
            _workContext = workContext;
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var ordersNode = rootNode.ChildNodes.Single(w => w.SystemName == "Sales");

            var perm = _permissionService.GetPermissionRecordBySystemName("VendorComissions");
            var visible = _workContext.CurrentCustomer.CustomerRoles.SelectMany(s => s.PermissionRecordCustomerRoleMappings.Where(w => w.PermissionRecordId == perm.Id)).Any();

            var subMenuItem = new SiteMapNode()
            {
                SystemName = "VendorComissions",
                Title = _localizationService.GetResource("VendorPercentage.MenuTitle"),
                ControllerName = "ComissionCalculator",
                ActionName = "VendorsList",
                IconClass = "fa-dot-circle-o",
                Visible = visible,
                RouteValues = new RouteValueDictionary() { { "area", "Admin" } },
            };

            ordersNode.ChildNodes.Add(subMenuItem);
        }


        public override void Install()
        {
            //locales
            _localizationService.AddOrUpdatePluginLocaleResource("VendorPercentage.MenuTitle", "Vendors comission");
            _localizationService.AddOrUpdatePluginLocaleResource("VendorPercentage.VendorsList.Title", "Vendors comission");
            _localizationService.AddOrUpdatePluginLocaleResource("VendorPercentage.VendorsList.DateFrom", "Date from");
            _localizationService.AddOrUpdatePluginLocaleResource("VendorPercentage.VendorsList.DateFrom.hint", "The start date the order received the status Completed.");
            _localizationService.AddOrUpdatePluginLocaleResource("VendorPercentage.VendorsList.DateTo", "Date to");
            _localizationService.AddOrUpdatePluginLocaleResource("VendorPercentage.VendorsList.DateTo.hint", "The final date the order received the status Completed.");
            _localizationService.AddOrUpdatePluginLocaleResource("VendorPercentage.VendorsList.Dates.hint", "Date the order received the status Completed");
            _localizationService.AddOrUpdatePluginLocaleResource("VendorPercentage.VendorsList.VendorName", "Vendor Name");
            _localizationService.AddOrUpdatePluginLocaleResource("VendorPercentage.VendorsList.VendorId", "ID");
            _localizationService.AddOrUpdatePluginLocaleResource("VendorPercentage.VendorsList.VendorComissionPercentage", "Comission %");
            _localizationService.AddOrUpdatePluginLocaleResource("VendorPercentage.VendorsList.VendorTotalAmountOfSales", "Total amount of vendor sales");
            _localizationService.AddOrUpdatePluginLocaleResource("VendorPercentage.VendorsList.VendorComission", "Comission Amount");
            _localizationService.AddOrUpdatePluginLocaleResource("VendorPercentage.VendorsList.VendorTransaction", "Transaction");
            _localizationService.AddOrUpdatePluginLocaleResource("VendorPercentage.VendorsList.FreeShippingSum", "Free Shipping");

            _permissionService.InstallPermissions(new Permission());

            base.Install();
        }

        public override void Uninstall()
        {
            //locales
            _localizationService.DeletePluginLocaleResource("VendorPercentage.MenuTitle");
            _localizationService.DeletePluginLocaleResource("VendorPercentage.VendorsList.Title");
            _localizationService.DeletePluginLocaleResource("VendorPercentage.VendorsList.DateFrom");
            _localizationService.DeletePluginLocaleResource("VendorPercentage.VendorsList.DateFrom.hint");
            _localizationService.DeletePluginLocaleResource("VendorPercentage.VendorsList.DateTo");
            _localizationService.DeletePluginLocaleResource("VendorPercentage.VendorsList.DateTo.hint");
            _localizationService.DeletePluginLocaleResource("VendorPercentage.VendorsList.Dates.hint");

            _localizationService.DeletePluginLocaleResource("VendorPercentage.VendorsList.VendorName");
            _localizationService.DeletePluginLocaleResource("VendorPercentage.VendorsList.VendorId");
            _localizationService.DeletePluginLocaleResource("VendorPercentage.VendorsList.VendorComissionPercentage");
            _localizationService.DeletePluginLocaleResource("VendorPercentage.VendorsList.VendorTotalAmountOfSales");
            _localizationService.DeletePluginLocaleResource("VendorPercentage.VendorsList.VendorComission");
            _localizationService.DeletePluginLocaleResource("VendorPercentage.VendorsList.VendorTransaction");
            _localizationService.DeletePluginLocaleResource("VendorPercentage.VendorsList.FreeShippingSum");

            _permissionService.UninstallPermissions(new Permission());

            base.Uninstall();

        }
    }
}
