using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tasks;
using Nop.Plugin.Shipping.Speedy.Data;
using Nop.Plugin.Shipping.Speedy.Models;
using Nop.Plugin.Shipping.Speedy.Services;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;
using Nop.Services.Tasks;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.Shipping.Speedy
{
    public class SpeedyComputationMethod : BasePlugin, IShippingRateComputationMethod, IWidgetPlugin, IScheduleTask, IAdminMenuPlugin
    {

        private readonly SpeedyShipmentObjectContext _objectContext;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly ISpeedyShipmentsService _speedySrv;
        private readonly WidgetSettings _widgetSettings;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        public SpeedyComputationMethod(
            ISpeedyShipmentsService speedySrv,
            SpeedyShipmentObjectContext objectContext,
            IScheduleTaskService scheduleTaskService,
            WidgetSettings widgetSettings,
            ISettingService settingService,
            ILocalizationService localizationService,
            IWebHelper webHelper, IWorkContext workContext, IStoreContext storeContext, IGenericAttributeService genericAttributeService)
        {
            _speedySrv = speedySrv;
            _objectContext = objectContext;
            _scheduleTaskService = scheduleTaskService;
            _widgetSettings = widgetSettings;
            _settingService = settingService;
            _localizationService = localizationService;
            _webHelper = webHelper;
            _workContext = workContext;
            _storeContext = storeContext;
            _genericAttributeService = genericAttributeService;
        }


        public ShippingRateComputationMethodType ShippingRateComputationMethodType => ShippingRateComputationMethodType.Realtime;

        public GetShippingOptionResponse GetShippingOptions(GetShippingOptionRequest getShippingOptionRequest)
        {
            var response = new GetShippingOptionResponse();
            response.ShippingOptions.Add(new ShippingOption
            {
                Name = _localizationService.GetResource("Speedy.ShippingMethod.Title"),
                Description = _localizationService.GetResource("Speedy.ShippingMethod.Description"),
                Rate = GetFixedRate(getShippingOptionRequest) ?? 0
            });

            return response;
        }

        public decimal? GetFixedRate(GetShippingOptionRequest getShippingOptionRequest)
        {
            var bolInfo = _genericAttributeService.GetAttribute<SpeedyBolInfo>(_workContext.CurrentCustomer, SpeedyDefaults.SpeedyShipmentConfiguresAttribute, _storeContext.CurrentStore.Id);
            if (bolInfo == null)
            {
                if (!string.IsNullOrEmpty(getShippingOptionRequest.ShippingAddress.ZipPostalCode))
                {
                    var site = _speedySrv.GetSiteInfoByBarCode(getShippingOptionRequest.ShippingAddress.ZipPostalCode);
                    if (site != null)
                        return _speedySrv.CalculateShippingByCartAndBol(505, getShippingOptionRequest.Items.Where(w => !w.ShoppingCartItem.Product.IsFreeShipping).ToList(), null, site.id);
                }
                return null;
            }

            return _speedySrv.CalculateShippingByCartAndBol(bolInfo.ServiceId, getShippingOptionRequest.Items.ToList(), bolInfo.OfficeId, bolInfo.SiteId);
        }

        public IShipmentTracker ShipmentTracker => new SpeedyTracker(_speedySrv);

        public bool HideInWidgetList { get; }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/Speedy/Configure";
        }


        public IList<string> GetWidgetZones()
        {
            return new[]
            {
                PublicWidgetZones.OpCheckoutShippingMethodBottom,
                PublicWidgetZones.CheckoutShippingMethodTop
            };
        }

        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "WidgetsSpeedy";
        }


        public void Execute()
        {
            _speedySrv.RunCheckingShipmentsList();
        }
        

        public override void Install()
        {
            //database objects
            _objectContext.Install();
            _scheduleTaskService.InsertTask(new ScheduleTask
            {
                Enabled = true,
                Type = typeof(SpeedyComputationMethod).FullName,
                Name = "Creating Speedy BOL",
                StopOnError = false,
                Seconds = 60

            });

            //locales
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.ShippingMethod.Title", "Speedy shipping");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.ShippingMethod.Description", "Shipping over Speedy");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.AddressForm.LoadingFormData", "Data loading");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.AddressForm.StreetName", "StreetName");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.AddressForm.Quarter", "Quarter");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.AddressForm.StreetNo", "Street No");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.AddressForm.BlockNo", "Block No");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.AddressForm.EntranceNo", "Entrance No");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.AddressForm.FloorNo", "Floor No");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.AddressForm.ApNumber", "Ap Number");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.AddressForm.City", "City");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.AddressForm.ShippingOver", "Shipping Over");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.AddressForm.Comment", "Comment");

            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.AddressForm.ToAutomat", "ToAutomat");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.AddressForm.ToOffice", "To Office");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.AddressForm.ToAddress", "To Address");

            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.AddressForm.Save", "Save");

            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.AddressForm.UnsavedFormError", "Please, save form data!");


            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.ShipmentsList.SpeedyShippingTitle", "Speedy shipping");
            _localizationService.AddOrUpdatePluginLocaleResource("Speydy.ShipmentsList", "Shipments list");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.ShipmentsList.RequestForCourier", "Request for Courier");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.ShipmentsList.BarCode", "BarCode");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.ShipmentsList.OrderId", "Order ID");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.ShipmentsList.CustomerName", "Customer Name");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.ShipmentsList.DateCreated", "Date Created");

            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.ShipmentsList.BolCreatingStatus", "Creating status");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.ShipmentsList.BolCreatingStatus.Pending", "Pending");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.ShipmentsList.BolCreatingStatus.BolIsCreated", "BolIsCreated");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.ShipmentsList.BolCreatingStatus.ErrorBolCreating", "ErrorBolCreating");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.ShipmentsList.BolCreatingStatus.Cancelled", "Cancelled");


            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.ShipmentsList.CourierStatus", "Courier Status");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.ShipmentsList.CourierStatus.NotRequested", "NotRequested");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.ShipmentsList.CourierStatus.Requested", "Requested");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.ShipmentsList.CourierStatus.BringToOffice", "BringToOffice");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.ShipmentsList.CourierStatus.ErrorRequested", "ErrorRequested");

            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.ShipmentsList.CancelBolConfirmTitle", "BillOfLading cancelling");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.ShipmentsList.CancelBolConfirmMessage", "You want to cancel BillOfLading. Type comment, please!");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.ShipmentsList.ErrorOperationTitle", "Operation Error!");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.ShipmentsList.ErrorOperationMessage", "An error occurred during the operation. Check the logs, please!");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.ShipmentsList.CourierPickingConfirmTitle", "Courier picking");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.ShipmentsList.CourierPickingConfirmMessage", "Courier picking. Input nessesary data");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.ShipmentsList.CourierPickingDate", "Picking Date");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.ShipmentsList.CourierPickingContactName", "Contact Name");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.ShipmentsList.CourierPickingPhoneNumber", "Phone Number");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.ShipmentsList.Cancel", "Cancel");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.ShipmentsList.PickCourierBtnText", "Pick the courier");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.ShipmentsList.CancelBtnText", "Cancel");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.ShipmentsList.CloseBtnText", "Close");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.ShipmentsList.RequestForCourier", "Request For Courier");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.ShipmentsList.ShippingCost", "Shipping Cost");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.ShipmentsList.CodComission", "CoD comission");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.ShipmentsList.IsFreeShipping", "Is FreeShipping");

            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.Admin.ConfigurationFields.Login", "Login");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.Admin.ConfigurationFields.Password", "Password");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.Admin.ConfigurationFields.DefaultWeight", "Default weight");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.Admin.ConfigurationFields.DefaultContent", "Default content");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.Admin.ConfigurationFields.DefaultPackage", "Default package");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.Admin.ConfigurationFields.DefaultPackage", "Sender phone number");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.Admin.ConfigurationFields.UseInsurance", "Use Insurance");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.Admin.ConfigurationFields.OptionsOpen", "Use Open option");
            _localizationService.AddOrUpdatePluginLocaleResource("Speedy.Admin.ConfigurationFields.OptionsTest", "Use Test option");





            //mark as active
            _widgetSettings.ActiveWidgetSystemNames.Add(this.PluginDescriptor.SystemName);
            _settingService.SaveSetting(_widgetSettings);

            base.Install();

        }

        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            _objectContext.Uninstall();

            var task = _scheduleTaskService.GetTaskByType(typeof(SpeedyComputationMethod).FullName);
            if (task != null)
                _scheduleTaskService.DeleteTask(task);


            //locales
            _localizationService.DeletePluginLocaleResource("Speedy.ShippingMethod.Title");
            _localizationService.DeletePluginLocaleResource("Speedy.ShippingMethod.Description");
            _localizationService.DeletePluginLocaleResource("Speedy.AddressForm.LoadingFormData");
            _localizationService.DeletePluginLocaleResource("Speedy.AddressForm.StreetName");
            _localizationService.DeletePluginLocaleResource("Speedy.AddressForm.Quarter");
            _localizationService.DeletePluginLocaleResource("Speedy.AddressForm.StreetNo");
            _localizationService.DeletePluginLocaleResource("Speedy.AddressForm.BlockNo");
            _localizationService.DeletePluginLocaleResource("Speedy.AddressForm.EntranceNo");
            _localizationService.DeletePluginLocaleResource("Speedy.AddressForm.FloorNo");
            _localizationService.DeletePluginLocaleResource("Speedy.AddressForm.ApNumber");
            _localizationService.DeletePluginLocaleResource("Speedy.AddressForm.City");
            _localizationService.DeletePluginLocaleResource("Speedy.AddressForm.ShippingOver");

            _localizationService.DeletePluginLocaleResource("Speedy.AddressForm.ToAutomat");
            _localizationService.DeletePluginLocaleResource("Speedy.AddressForm.ToOffice");
            _localizationService.DeletePluginLocaleResource("Speedy.AddressForm.ToAddress");
            _localizationService.DeletePluginLocaleResource("Speedy.AddressForm.Comment");

            _localizationService.DeletePluginLocaleResource("Speedy.AddressForm.Save");

            _localizationService.DeletePluginLocaleResource("Speedy.AddressForm.UnsavedFormError");


            _localizationService.DeletePluginLocaleResource("Speedy.ShipmentsList.SpeedyShippingTitle");
            _localizationService.DeletePluginLocaleResource("Speydy.ShipmentsList");
            _localizationService.DeletePluginLocaleResource("Speedy.ShipmentsList.RequestForCourier");
            _localizationService.DeletePluginLocaleResource("Speedy.ShipmentsList.BarCode");
            _localizationService.DeletePluginLocaleResource("Speedy.ShipmentsList.OrderId");
            _localizationService.DeletePluginLocaleResource("Speedy.ShipmentsList.CustomerName");
            _localizationService.DeletePluginLocaleResource("Speedy.ShipmentsList.DateCreated");
            _localizationService.DeletePluginLocaleResource("Speedy.ShipmentsList.BolCreatingStatus");
            _localizationService.DeletePluginLocaleResource("Speedy.ShipmentsList.CourierStatus");

            _localizationService.DeletePluginLocaleResource("Speedy.ShipmentsList.BolCreatingStatus.Pending");
            _localizationService.DeletePluginLocaleResource("Speedy.ShipmentsList.BolCreatingStatus.BolIsCreated");
            _localizationService.DeletePluginLocaleResource("Speedy.ShipmentsList.BolCreatingStatus.ErrorBolCreating");
            _localizationService.DeletePluginLocaleResource("Speedy.ShipmentsList.BolCreatingStatus.Cancelled");

            _localizationService.DeletePluginLocaleResource("Speedy.ShipmentsList.CourierStatus.NotRequested");
            _localizationService.DeletePluginLocaleResource("Speedy.ShipmentsList.CourierStatus.Requested");
            _localizationService.DeletePluginLocaleResource("Speedy.ShipmentsList.CourierStatus.BringToOffice");
            _localizationService.DeletePluginLocaleResource("Speedy.ShipmentsList.CourierStatus.ErrorRequested");

            _localizationService.DeletePluginLocaleResource("Speedy.ShipmentsList.CancelBolConfirmTitle");
            _localizationService.DeletePluginLocaleResource("Speedy.ShipmentsList.CancelBolConfirmMessage");
            _localizationService.DeletePluginLocaleResource("Speedy.ShipmentsList.ErrorOperationTitle");
            _localizationService.DeletePluginLocaleResource("Speedy.ShipmentsList.ErrorOperationMessage");
            _localizationService.DeletePluginLocaleResource("Speedy.ShipmentsList.CourierPickingConfirmTitle");
            _localizationService.DeletePluginLocaleResource("Speedy.ShipmentsList.CourierPickingConfirmMessage");
            _localizationService.DeletePluginLocaleResource("Speedy.ShipmentsList.CourierPickingDate");
            _localizationService.DeletePluginLocaleResource("Speedy.ShipmentsList.CourierPickingContactName");
            _localizationService.DeletePluginLocaleResource("Speedy.ShipmentsList.CourierPickingPhoneNumber");
            _localizationService.DeletePluginLocaleResource("Speedy.ShipmentsList.Cancel");
            _localizationService.DeletePluginLocaleResource("Speedy.ShipmentsList.PickCourierBtnText");
            _localizationService.DeletePluginLocaleResource("Speedy.ShipmentsList.CloseBtnText");
            _localizationService.DeletePluginLocaleResource("Speedy.ShipmentsList.CancelBtnText");
            _localizationService.DeletePluginLocaleResource("Speedy.ShipmentsList.RequestForCourier");
            _localizationService.DeletePluginLocaleResource("Speedy.ShipmentsList.ShippingCost");
            _localizationService.DeletePluginLocaleResource("Speedy.ShipmentsList.CodComission");
            _localizationService.DeletePluginLocaleResource("Speedy.ShipmentsList.IsFreeShipping");

            _localizationService.DeletePluginLocaleResource("Speedy.Admin.ConfigurationFields.Login");
            _localizationService.DeletePluginLocaleResource("Speedy.Admin.ConfigurationFields.Password");
            _localizationService.DeletePluginLocaleResource("Speedy.Admin.ConfigurationFields.DefaultWeight");
            _localizationService.DeletePluginLocaleResource("Speedy.Admin.ConfigurationFields.DefaultContent");
            _localizationService.DeletePluginLocaleResource("Speedy.Admin.ConfigurationFields.DefaultPackage");
            _localizationService.DeletePluginLocaleResource("Speedy.Admin.ConfigurationFields.DefaultPackage");
            _localizationService.DeletePluginLocaleResource("Speedy.Admin.ConfigurationFields.UseInsurance");
            _localizationService.DeletePluginLocaleResource("Speedy.Admin.ConfigurationFields.OptionsOpen");
            _localizationService.DeletePluginLocaleResource("Speedy.Admin.ConfigurationFields.OptionsTest");

            base.Uninstall();
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {

            var menuItemBuilder = new SiteMapNode()
            {
                Title = "Speedy",   // Title for your Custom Menu Item
                //Url = "Path of action link", // Path of the action link
                IconClass = "fa-truck",
                Visible = true,
                RouteValues = new RouteValueDictionary() { { "Area", "Admin" } }
            };

            var subMenuItem = new SiteMapNode()
            {
                SystemName = "SpeedyShipments",
                Title = "Speedy shipments list",
                ControllerName = "Speedy",
                ActionName = "ShipmentsList",
                IconClass = "fa-dot-circle-o",
                Visible = true,
                RouteValues = new RouteValueDictionary() { { "area", "Admin" } },
            };

            menuItemBuilder.ChildNodes.Add(subMenuItem);

            rootNode.ChildNodes.Add(menuItemBuilder);
        }
    }
}
