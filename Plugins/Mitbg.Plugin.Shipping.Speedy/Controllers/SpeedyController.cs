using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mitbg.Plugin.Misc.VendorsCore;
using Mitbg.Plugin.Misc.VendorsCore.Domain;
using Mitbg.Plugin.Misc.VendorsCore.Domain.Entities;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Shipping.Speedy.Domain;
using Nop.Plugin.Shipping.Speedy.Models;
using Nop.Plugin.Shipping.Speedy.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Tasks;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Shipping.Speedy.Controllers
{
    public class SpeedyController : BasePluginController
    {
        private readonly IShipmentContext _speedyContext;
        private readonly IWorkContext _workContext;
        private readonly IRepository<ShipmentTask> _shipmentTasksRep;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ISpeedyShipmentsService _speedyService;
        private readonly IShippingService _shippingService;
        private readonly SpeedySettings _speedySettings;
        private readonly ISettingService _settingService;
        private readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IStoreContext _storeContext;
        private readonly IRepository<Address> _addressesRep;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;

        private static string sessionGuid = string.Empty;
        private static resultLogin login = null;

        private Binding binding = new BasicHttpsBinding(BasicHttpsSecurityMode.Transport)
        {
            MaxReceivedMessageSize = int.MaxValue
        };
        private EndpointAddress address = new EndpointAddress("https://www.speedy.bg/eps/mainservice01");

        public SpeedyController(
            IWorkContext workContext,
            IShipmentContext speedyContext,
            IRepository<ShipmentTask> shipmentTasksRep,
            ISpeedyShipmentsService speedyService,
            SpeedySettings speedySettings,
            ISettingService settingService,
            ILocalizationService localizationService,
            INotificationService notificationService, IGenericAttributeService genericAttributeService, IStoreContext storeContext, IShippingService shippingService, IShoppingCartService shoppingCartService, IRepository<Address> addressesRep, IPriceFormatter priceFormatter, ICurrencyService currencyService, CurrencySettings currencySettings)
        {
            _workContext = workContext;
            _speedyContext = speedyContext;
            _shipmentTasksRep = shipmentTasksRep;
            _speedyService = speedyService;
            _speedySettings = speedySettings;
            _settingService = settingService;
            _notificationService = notificationService;
            _genericAttributeService = genericAttributeService;
            _storeContext = storeContext;
            _shippingService = shippingService;
            _shoppingCartService = shoppingCartService;
            _addressesRep = addressesRep;
            _priceFormatter = priceFormatter;
            _currencyService = currencyService;
            _currencySettings = currencySettings;
            _localizationService = localizationService;
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            //prepare common properties
            var model = new ConfigurationModel
            {
                Login = _speedySettings.Login,
                Password = _speedySettings.Password,
                DefaultContent = _speedySettings.DefaultContent,
                DefaultPackage = _speedySettings.DefaultPackage,
                DefaultWeight = _speedySettings.DefaultWeight,
                SenderPhoneNumber = _speedySettings.SenderPhoneNumber,
                UseInsurance = _speedySettings.UseInsurance,
                OptionsOpen = _speedySettings.OptionsOpen,
                OptionsTest = _speedySettings.OptionsTest,
                CodMethod = _speedySettings.CodMethod
            };


            return View("~/Plugins/Mitbg.Plugin.Shipping.Speedy/Views/Configuration.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            //save settings
            _speedySettings.Login = model.Login;
            _speedySettings.Password = model.Password;
            _speedySettings.DefaultWeight = model.DefaultWeight;
            _speedySettings.DefaultContent = model.DefaultContent;
            _speedySettings.DefaultPackage = model.DefaultPackage;
            _speedySettings.SenderPhoneNumber = model.SenderPhoneNumber;
            _speedySettings.UseInsurance = model.UseInsurance;
            _speedySettings.OptionsOpen = model.OptionsOpen;
            _speedySettings.OptionsTest = model.OptionsTest;
            _speedySettings.CodMethod = model.CodMethod;


            _settingService.SaveSetting(_speedySettings);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }


        [Area(AreaNames.Admin)]
        [HttpGet]
        public IActionResult ShipmentsList()
        {
            var model = new SpeedyShipmentsSearchModel();
            return View("~/Plugins/Mitbg.Plugin.Shipping.Speedy/Views/ShipmentsList.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery]
        public IActionResult ShipmentsList(SpeedyShipmentsSearchModel searchModel)
        {
            var shipmentsQuery = _shipmentTasksRep.Table;
            if (searchModel.DateCreated.HasValue)
            {
                shipmentsQuery = shipmentsQuery.Where(w => w.DateCreated.Date == searchModel.DateCreated.Value.Date);
            }

            if (searchModel.BarCode.HasValue)
            {
                shipmentsQuery = shipmentsQuery.Where(w => w.BarCode == searchModel.BarCode.ToString());
            }
            if (searchModel.OrderId.HasValue)
            {
                shipmentsQuery = shipmentsQuery.Where(w => w.OrderId == searchModel.OrderId);
            }

            if (!string.IsNullOrEmpty(searchModel.CustomerName))
            {
                shipmentsQuery = shipmentsQuery.Where(w => w.CustomerName.Contains(searchModel.CustomerName));
            }

            if (_workContext.CurrentVendor != null)
            {
                //shipmentsQuery = from shipment in shipmentsQuery
                //                 join order in _ordersRep.Table on shipment.OrderId equals order.Id
                //                 where order.Customer.VendorId == _workContext.CurrentCustomer.VendorId
                //                 select shipment;

                shipmentsQuery =
                    shipmentsQuery.Where(w => w.VendorId == _workContext.CurrentCustomer.VendorId);
            }
            var primaryStoreCurrency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);

            var shipments = shipmentsQuery.ToList().Select(s => new SpeedyShipmentModel
            {
                Id = s.Id,
                CustomerName = s.CustomerName,
                DateCreated = s.DateCreated,
                IsFreeShipping = s.IsFreeShipping,
                ShippingCostText = _priceFormatter.FormatPrice(s.ShippingCost, true, primaryStoreCurrency, _workContext.WorkingLanguage, false, false),
                CodComissionText = _priceFormatter.FormatPrice(s.CodComission, true, primaryStoreCurrency, _workContext.WorkingLanguage, false, false),
                OrderId = s.OrderId,
                BarCode = s.BarCode,
                BolCreatingStatus = s.BolCreatingStatus,
                BolCreatingStatusText = _localizationService.GetResource("Speedy.ShipmentsList.BolCreatingStatus." + s.BolCreatingStatus),
                CourierStatus = s.CourierStatus,
                CourierStatusText = _localizationService.GetResource("Speedy.ShipmentsList.CourierStatus." + s.CourierStatus),

                ErrorMessage = s.BolCreatingStatus == BolCreatingStatus.ErrorBolCreating ? s.BolCreatingErrorMessage : string.Empty

            }).OrderByDescending(w => w.Id);

            var totalCount = shipmentsQuery.Count();
            var model = new SpeedyShipmentListModel
            {
                Data = shipments,
                Total = totalCount,
                RecordsTotal = totalCount,
                RecordsFiltered = totalCount
            };

            return Json(model);
        }

        [HttpPost]
        [AdminAntiForgery]
        public IActionResult CancelShipping(int shipmentId, string message)
        {
            _speedyService.CancelBol(shipmentId, message);
            return Ok();
        }

        [HttpPost]
        [AdminAntiForgery]
        public IActionResult CourierPicking(int[] shipmentIds, DateTime? datePick, string contactName, string phoneNumber)
        {
            _speedyService.RequestCourier(shipmentIds, datePick, contactName, phoneNumber);
            return Ok();
        }


        public async Task<ActionResult> GetSites(string term)
        {
            var customer = _workContext.CurrentCustomer;
            var bol = new ShipmentInfo();
            _speedyContext.SetBillInfo(customer.Id, bol);

            using (var srv = GetSpeedyClient())
            {

                var sites = (await srv.listSitesAsync(login.sessionId, "", term, paramLanguage.BG)).@return;

                return Json(new
                {
                    results = sites != null && sites.Any() ? sites.Select(s => new
                    {
                        s.id,
                        text = string.Format("{0}{1}({2}), общ.{3}, обл.{4}", s.type, s.name, s.postCode, s.municipality, s.region)
                    }).ToArray() : new object[] { },
                    pagination = new
                    {
                        more = false
                    }
                });
            }
        }
        public async Task<ActionResult> GetOfficies(int siteId, bool isApt)
        {
            using (var srv = GetSpeedyClient())
            {
                var sites = await srv.listOfficesExAsync(login.sessionId, "", siteId, paramLanguage.BG, 100);

                return Json(new
                {
                    results = sites.@return.Where(w => w.officeType == (isApt ? 3 : 0)).Select(s => new
                    {
                        s.id,
                        text = string.Format("{0} {1}, {2}", s.id, s.name, s.address.fullAddressString)
                    }).ToArray(),
                    pagination = new
                    {
                        more = false
                    }
                });
            }
        }

        public async Task<ActionResult> GetQuarters(string term)
        {
            var customer = _workContext.CurrentCustomer;
            var bol = _speedyContext.GetBillInfo(customer.Id) ?? new ShipmentInfo();

            using (var srv = GetSpeedyClient())
            {
                var queaters = (await srv.listQuartersAsync(login.sessionId, term, bol.SiteId, paramLanguage.BG)).@return;

                return Json(new
                {
                    results = queaters != null && queaters.Any() ? queaters.Select(s => new
                    {
                        s.id,
                        text = string.Format("{0} {1}", s.type, s.name).Trim(),
                        s.name,
                        s.type
                    }).ToArray() : new object[] { },
                    pagination = new
                    {
                        more = false
                    }
                });
            }
        }

        public async Task<ActionResult> GetStreets(string term)
        {
            var customer = _workContext.CurrentCustomer;
            var bol = _speedyContext.GetBillInfo(customer.Id) ?? new ShipmentInfo();


            using (var srv = GetSpeedyClient())
            {
                var streets = (await srv.listStreetsAsync(login.sessionId, term, bol.SiteId, paramLanguage.BG)).@return;

                return Json(new
                {
                    results = streets != null && streets.Any() ? streets.Select(s => new
                    {
                        s.id,
                        text = string.Format("{0} {1}", s.type, s.name).Trim()
                    }).ToArray() : new object[] { },
                    pagination = new
                    {
                        more = false
                    }
                });
            }
        }

        [AdminAntiForgery]
        public async Task<IActionResult> CreatePdf(long barCode)
        {
            using (var srv = GetSpeedyClient())
            {
                var pdfFile = await srv.createPDFAsync(login.sessionId, new paramPDF
                {
                    type = 10,
                    ids = new long?[] { barCode }
                });

                return File(new MemoryStream(pdfFile.@return), "application/pdf", string.Format("{0}.pdf", barCode));
            }
        }


        public async Task<ActionResult> SpeedyForm(FormViewModel model)
        {
            var customer = _workContext.CurrentCustomer;
            var bol = _speedyContext.GetBillInfo(customer.Id) ?? new ShipmentInfo();

            model.DisableAutomats = true;
            model.DisableOfficies = false;

            if (model.SiteId != bol.SiteId || bol.AllOfficies == null)
            {
                var listOfficies = await LoadOfficies(model.SiteId);

                bol.AllOfficies = listOfficies;

                //model.DisableAutomats = !listOfficies.Any(w => w.IsAutomat);
                //model.DisableOfficies = listOfficies.All(w => w.IsAutomat);

                bol.SiteId = model.SiteId;
                bol.SiteName = model.SiteName;

                if (model.DisableAutomats && model.DeliveryOption == DeliveryOption.Automat)
                    model.DeliveryOption = DeliveryOption.Office;

                if (model.DisableOfficies && model.DeliveryOption == DeliveryOption.Office)
                    model.DeliveryOption = model.DisableAutomats ? DeliveryOption.Address : DeliveryOption.Automat;


                model.OfficeId = -1;
                bol.Officies = null;

            }

            switch (model.DeliveryOption)
            {
                case DeliveryOption.Address:
                    model.AddressNote = "";
                    model.StreetName = "";
                    model.StreetNo = "";
                    model.QuarterText = "";
                    model.QuarterName = "";
                    model.QuarterType = "";
                    model.QuarterId = 0;
                    model.Block = "";
                    model.Entrance = "";
                    model.Floor = "";

                    break;

                case DeliveryOption.Automat:
                case DeliveryOption.Office:

                    bol.Officies = new List<KeyValuePair<long, string>>() { new KeyValuePair<long, string>(-1, "Not selected") }
                        .Union(bol.AllOfficies.Where(w => w.IsAutomat == (model.DeliveryOption == DeliveryOption.Automat))
                            .Select(s => new KeyValuePair<long, string>(s.Id, s.Name))).ToList();

                    model.Officies = bol.Officies;

                    if (model.OfficeId > 0 && (model.OfficeId != bol.OfficeId))
                    {
                        bol.OfficeId = model.OfficeId;
                        var office = bol.Officies.Single(s => s.Key == bol.OfficeId);
                        model.AddressNote = office.Value;

                    }
                    break;
            }

            bol.SiteId = model.SiteId;
            bol.SiteName = model.SiteName;

            bol.AddressNote = model.AddressNote;
            bol.StreetName = model.AddressNote;
            bol.StreetNo = model.StreetNo;
            bol.QuarterName = model.QuarterName;
            bol.QuarterText = model.QuarterText;
            bol.QuarterType = model.QuarterType;
            bol.QuarterId = model.QuarterId;
            bol.Block = model.Block;
            bol.Entrance = model.Entrance;
            bol.Floor = model.Floor;

            _speedyContext.SetBillInfo(customer.Id, bol);

            return View("~/Plugins/Mitbg.Plugin.Shipping.Speedy/Views/_AddressForm.cshtml", model);
        }

        private async Task<List<CargoOffice>> LoadOfficies(int siteId)
        {
            using (var srv = GetSpeedyClient())
            {
                var sites = await srv.listOfficesExAsync(login.sessionId, "", siteId, paramLanguage.BG, 100);


                return sites.@return != null ? sites.@return.Select(s =>
                       new CargoOffice(
                           s.id,
                           string.Format("{0} {1}, {2}", s.id, s.name, s.address.fullAddressString),
                           s.officeType == 3)
                ).ToList() : new List<CargoOffice>();
            }
        }

        public ActionResult SaveForm(FormViewModel model)
        {
            var customer = _workContext.CurrentCustomer;

            var bol = _speedyContext.GetBillInfo(customer.Id) ?? new ShipmentInfo();

            bol.SiteId = model.SiteId;
            bol.SiteName = model.SiteName;
            bol.CountryId = 100;
            bol.OfficeId = model.DeliveryOption == DeliveryOption.Address ? 0 : model.OfficeId;
            bol.StreetName = model.StreetName;
            bol.StreetNo = model.StreetNo;

            bol.QuarterName = model.QuarterName;
            bol.QuarterText = model.QuarterText;
            bol.QuarterType = model.QuarterType;
            bol.QuarterId = model.QuarterId;

            bol.Block = model.Block;
            bol.Entrance = model.Entrance;
            bol.ApNumber = model.ApNumber;
            bol.AddressNote = model.AddressNote;
            bol.DeliveryOption = model.DeliveryOption;
            bol.Comment = model.Comment;

            model.Officies = bol.Officies;

            _speedyContext.SetBillInfo(customer.Id, bol);
            _genericAttributeService.SaveAttribute(customer, SpeedyDefaults.SpeedyShipmentConfiguresAttribute, bol, _storeContext.CurrentStore.Id);

            model.IsSaved = true;

            var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id);

            var splittedOption = "Speedy shipping___Shipping.Speedy".Split(new[] { "___" }, StringSplitOptions.RemoveEmptyEntries);
            var selectedName = splittedOption[0];
            var shippingRateComputationMethodSystemName = splittedOption[1];

            var shippingOptions = _genericAttributeService.GetAttribute<List<ShippingOption>>(_workContext.CurrentCustomer,
                NopCustomerDefaults.OfferedShippingOptionsAttribute, _storeContext.CurrentStore.Id);
            if (shippingOptions == null || !shippingOptions.Any())
            {
                //not found? let's load them using shipping service
                shippingOptions = _shippingService.GetShippingOptions(cart, _workContext.CurrentCustomer.ShippingAddress,
                    _workContext.CurrentCustomer, shippingRateComputationMethodSystemName, _storeContext.CurrentStore.Id).ShippingOptions.ToList();
            }
            else
            {
                //loaded cached results. let's filter result by a chosen shipping rate computation method
                shippingOptions = shippingOptions.Where(so => so.ShippingRateComputationMethodSystemName.Equals(shippingRateComputationMethodSystemName, StringComparison.InvariantCultureIgnoreCase))
                    .ToList();
            }

            var shippingOption = shippingOptions
                .Find(so => !string.IsNullOrEmpty(so.Name) && so.Name.Equals(selectedName, StringComparison.InvariantCultureIgnoreCase));

            if (shippingOption != null)
            {

                //var attributes = _genericAttributeService.GetAttributesForEntity(customer.Id, "Customer");
                shippingOptions = shippingOptions
                        .Where(so => string.IsNullOrEmpty(so.Name) || !so.Name.Equals(selectedName, StringComparison.InvariantCultureIgnoreCase)).ToList();
                _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.OfferedShippingOptionsAttribute, shippingOptions, _storeContext.CurrentStore.Id);
            }

            var shippingAddress = customer.ShippingAddress;
            if (customer.BillingAddressId.HasValue && customer.BillingAddressId == customer.ShippingAddressId)
            {
                shippingAddress = new Address
                {
                    FirstName = shippingAddress.FirstName,
                    LastName = shippingAddress.LastName,
                    Address1 = shippingAddress.Address1,
                    Address2 = shippingAddress.Address2,
                    City = shippingAddress.City,
                    ZipPostalCode = shippingAddress.ZipPostalCode,
                    PhoneNumber = shippingAddress.PhoneNumber,
                    Email = shippingAddress.Email,
                    Company = shippingAddress.Company,
                    Country = shippingAddress.Country,
                    CustomAttributes = shippingAddress.CustomAttributes,
                };
                _addressesRep.Insert(shippingAddress);
                customer.ShippingAddress = shippingAddress;
            }

            shippingAddress.City = bol.SiteName;

            shippingAddress.Address1 =
                string.IsNullOrEmpty((bol.StreetName + bol.StreetNo).Trim()) ? "" :
            string.Format(@"{0}, № {1}", bol.StreetName, bol.StreetNo);

            if (!string.IsNullOrEmpty(bol.QuarterText))
                shippingAddress.Address2 = bol.QuarterText;

            if (!string.IsNullOrEmpty(bol.Block))
                shippingAddress.Address2 += ", Бл. " + bol.Block;

            if (!string.IsNullOrEmpty(bol.Entrance))
                shippingAddress.Address2 += ", Вх. " + bol.Entrance;

            if (!string.IsNullOrEmpty(bol.Floor))
                shippingAddress.Address2 += ", Ет. " + bol.Floor;

            if (!string.IsNullOrEmpty(bol.ApNumber))
                shippingAddress.Address2 += ", Ап. " + bol.ApNumber;

            customer.ShippingAddress = shippingAddress;
            _addressesRep.Update(shippingAddress);

            return View("~/Plugins/Mitbg.Plugin.Shipping.Speedy/Views/_AddressForm.cshtml", model);
        }


        private EPSProviderClient GetSpeedyClient()
        {
            var srv = new EPSProviderClient(binding, address);

            if (login == null || login.sessionId != sessionGuid || string.IsNullOrEmpty(sessionGuid) ||
                !srv.isSessionActive(sessionGuid, true))
            {
                login = srv.login(_speedySettings.Login, _speedySettings.Password);
                sessionGuid = login.sessionId;
            }

            if (login == null)
                throw new Exception("Login to Speedy unsuccessfull");

            return srv;

        }

    }
}

