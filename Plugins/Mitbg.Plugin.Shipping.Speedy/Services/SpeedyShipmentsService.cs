using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using Mitbg.Plugin.Misc.VendorsCore.Domain;
using Mitbg.Plugin.Misc.VendorsCore.Domain.Entities;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Vendors;
using Nop.Plugin.Shipping.Speedy.Domain;
using Nop.Services.Logging;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;
using NUglify.Helpers;


namespace Nop.Plugin.Shipping.Speedy.Services
{
    public class SpeedyShipmentsService : ISpeedyShipmentsService
    {

        private readonly ILogger _logger;
        private readonly IRepository<ShipmentTask> _shipmentTaskRep;
        private readonly IRepository<Order> _ordersRep;
        private readonly IRepository<Vendor> _vendorsRep;
        private readonly IRepository<Address> _addressesRep;
        private readonly IRepository<OrderItem> _orderItemsRep;
        private readonly IShipmentService _shipmentService;
        private readonly SpeedySettings _speedySettings;


        private Binding binding = new BasicHttpsBinding(BasicHttpsSecurityMode.Transport)
        {
            MaxReceivedMessageSize = int.MaxValue
        };
        private EndpointAddress address = new EndpointAddress("https://www.speedy.bg/eps/mainservice01");


        private static string sessionGuid = string.Empty;
        private static resultLogin login = null;

        public SpeedyShipmentsService(ILogger logger,
            IRepository<ShipmentTask> shipmentTaskRep,
            IRepository<Order> ordersRep,
            IRepository<OrderItem> orderItemsRep,
            IShipmentService shipmentService,
            SpeedySettings speedySettings, IRepository<Vendor> vendorsRep, IRepository<Address> addressesRep)
        {
            _logger = logger;
            _shipmentTaskRep = shipmentTaskRep;
            _ordersRep = ordersRep;
            _orderItemsRep = orderItemsRep;
            _shipmentService = shipmentService;
            _speedySettings = speedySettings;
            _vendorsRep = vendorsRep;
            _addressesRep = addressesRep;
        }

        public void RunCheckingShipmentsList()
        {
            //_logger.InsertLog(LogLevel.Debug, "Run SpeedyShipmentsService");
            var notCreatedBolItems = _shipmentTaskRep.Table.Where(w => w.BolCreatingStatus == BolCreatingStatus.Pending).ToList();
            //_logger.InsertLog(LogLevel.Debug, "Not cretead bol shipment count " + notCreatedBolItems.Count);
            foreach (var item in notCreatedBolItems)
            {
                CreateBol(item);
            }

        }

        public void CancelBol(int shipmentId, string message)
        {
            try
            {
                using (var srv = GetSpeedyClient())
                {

                    var shipment = _shipmentTaskRep.GetById(shipmentId);
                    if (shipment != null)
                    {
                        if (shipment.BolCreatingStatus == BolCreatingStatus.Pending || shipment.BolCreatingStatus == BolCreatingStatus.ErrorBolCreating)
                        {
                            shipment.BolCreatingStatus = BolCreatingStatus.Cancelled;
                            _shipmentTaskRep.Update(shipment);
                        }
                        else if (shipment.BolCreatingStatus == BolCreatingStatus.BolIsCreated)
                        {
                            try
                            {
                                srv.invalidatePicking(login.sessionId, long.Parse(shipment.BarCode), message);
                                shipment.BolCreatingStatus = BolCreatingStatus.Cancelled;
                                _shipmentTaskRep.Update(shipment);
                            }
                            catch (Exception e)
                            {
                                if (e.Message.StartsWith("[ERR_900]"))
                                {
                                    _logger.Warning($"Picking with bol: [{shipment.BarCode}] is already canceled", e);

                                    shipment.BolCreatingStatus = BolCreatingStatus.Cancelled;
                                    _shipmentTaskRep.Update(shipment);
                                }
                            }

                        }
                    }
                    else
                    {
                        throw new Exception(string.Format("Shipment with Id=[{0}] not found!", shipmentId));
                    }

                }
            }
            catch (Exception e)
            {
                _logger.Error(e.Message, e);
                throw;
            }
        }

        private void CreateBol(ShipmentTask shipment)
        {
            using (var srv = GetSpeedyClient())
            {
                var takingDate = DateTime.Today.AddDays(1).AddHours(9).AddMinutes(0);

                var allowedDaysForTaking = srv.getAllowedDaysForTaking(sessionGuid, shipment.ServiceId, 0, 0, takingDate, 0, null, login.clientId);

                takingDate = allowedDaysForTaking.Min();

                try
                {
                    var order = _ordersRep.Table.SingleOrDefault(w => w.Id == shipment.OrderId);
                    if (order == null)
                        throw new Exception(string.Format("Order with ID=[{0}] not found", shipment.OrderId));

                    if (!shipment.UseCod && order.PaymentStatus != PaymentStatus.Authorized && order.PaymentStatus != PaymentStatus.Paid)
                        return;


                    var orderItems = _orderItemsRep.Table.Where(w =>
                        w.OrderId == shipment.OrderId
                        && w.Product.VendorId == shipment.VendorId
                        && w.Product.IsFreeShipping == shipment.IsFreeShipping)
                        .ToList();

                    if (!orderItems.Any())
                        throw new Exception(string.Format("Positions for order with ID=[{0}] not found",
                            shipment.OrderId));


                    var amount = (double)orderItems.Sum(s => s.PriceInclTax);
                    amount += (double)order.OrderShippingInclTax;
                    amount += (double)order.PaymentMethodAdditionalFeeInclTax;

                    var weight = (double)orderItems.Sum(s => s.ItemWeight ?? 0);
                    var payerType = 0;

                    var sender = new paramClientData()
                    {
                        clientId = login.clientId,
                        clientIdSpecified = true,
                        phones = new[]
                        {
                            new paramPhoneNumber
                            {
                                number = _speedySettings.SenderPhoneNumber
                            }
                        }
                    };
                    if (shipment.VendorId > 0)
                    {
                        payerType = 2; //if vendor then payer type is third party

                        var vendor = _vendorsRep.GetById(shipment.VendorId);
                        var vendorAddress = _addressesRep.GetById(vendor.AddressId);
                        var siteId = srv.listSitesEx(login.sessionId, new paramFilterSite
                        {
                            postCode = vendorAddress.ZipPostalCode
                        }, paramLanguage.BG).First().site.id;
                        sender = new paramClientData()
                        {
                            clientIdSpecified = false,
                            partnerName = vendor.Name,
                            address = new paramAddress
                            {
                                siteIdSpecified = true,
                                siteId = siteId,
                                postCode = vendorAddress.ZipPostalCode,
                                addressNote = string.Format(@"{0}, {1}, {2}", vendorAddress.City, vendorAddress.Address1, vendorAddress.Address2)
                            },
                            phones = new[]
                            {
                                new paramPhoneNumber
                                {
                                    number = vendorAddress.PhoneNumber
                                }
                            }
                        };

                    }

                    var pickingData = new paramPicking()
                    {
                        officeToBeCalledId = shipment.OfficeId ?? 0,
                        officeToBeCalledIdSpecified = (shipment.OfficeId ?? 0) > 0,
                        takingDate = takingDate,
                        takingDateSpecified = true,
                        backDocumentsRequest = false,
                        backReceiptRequest = false,
                        willBringToOffice = false,

                        serviceTypeId = shipment.ServiceId,

                        parcelsCount = orderItems.Count,
                        weightDeclared = weight > 0 ? weight : _speedySettings.DefaultWeight,

                        contents = _speedySettings.DefaultContent ?? "GOODS",
                        packing = _speedySettings.DefaultPackage ?? "CARTON BOX",

                        documents = false,
                        fragile = false,
                        palletized = false,

                        payerType = payerType,                                                         //
                        payerTypeInsurance = payerType,
                        payerTypeInsuranceSpecified = _speedySettings.UseInsurance,
                        payerRefId = payerType == 2 ? login.clientId : 0,
                        payerRefIdSpecified = payerType == 2,
                        payerRefInsuranceId = payerType == 2 ? login.clientId : 0,
                        payerRefInsuranceIdSpecified = _speedySettings.UseInsurance && payerType == 2,
                        payerTypePackings = payerType,
                        payerTypePackingsSpecified = true,
                        payCodToThirdParty = payerType == 2,
                        payCodToThirdPartySpecified = payerType == 2,
                        amountInsuranceBase = amount,
                        amountInsuranceBaseSpecified = _speedySettings.UseInsurance,
                        amountCodBase = amount,
                        amountCodBaseSpecified = shipment.UseCod && _speedySettings.CodMethod == CodMethod.NP,

                        retMoneyTransferReqAmount = amount,
                        retMoneyTransferReqAmountSpecified = shipment.UseCod && _speedySettings.CodMethod == CodMethod.PPP,


                        optionsBeforePayment = shipment.DeliveryOption != DeliveryOption.Automat && shipment.UseCod && (_speedySettings.OptionsOpen || _speedySettings.OptionsTest) ? new paramOptionsBeforePayment
                        {

                            open = _speedySettings.OptionsOpen,
                            openSpecified = _speedySettings.OptionsOpen,
                            test = _speedySettings.OptionsTest,
                            testSpecified = _speedySettings.OptionsTest,
                            returnPayerType = 0, //1. Ако искате получателя на правата товарителницата да е платец и на връщащата в случай на отказ трябва да подадете в ParamOptionsBeforePayment - 0 Sender, защото при генериране на товарителница при отказ местата на подателя и получателя се разменя, т.е. подател става клиента на който сте изпратили пратката.
                            returnPayerTypeSpecified = true,
                            returnServiceTypeId = shipment.ServiceId,
                            returnServiceTypeIdSpecified = true
                        } : null,

                        sender = sender,
                        receiver = new paramClientData()
                        {
                            partnerName = shipment.CustomerName,
                            address = shipment.OfficeId.HasValue && shipment.OfficeId > 0 ? null : new paramAddress()
                            {
                                countryId = shipment.CountryId,
                                siteId = shipment.SiteId,
                                siteIdSpecified = true,
                                quarterId = shipment.QuarterId,
                                quarterIdSpecified = shipment.QuarterId > 0,
                                quarterName = shipment.QuarterId > 0 ? "" : shipment.QuarterName,
                                quarterType = shipment.QuarterId > 0 ? "" : shipment.QuarterType,
                                blockNo = shipment.Block,
                                entranceNo = shipment.Entrance,
                                floorNo = shipment.Floor,
                                apartmentNo = shipment.ApNumber,
                                streetName = shipment.StreetName,
                                streetNo = shipment.StreetNo,
                                addressNote = (shipment.Comment ?? "").Length > 5 ? shipment.Comment : string.Empty

                            },
                            phones = new[]
                            {
                                new paramPhoneNumber
                                {
                                    number = shipment.CustomerPhone
                                }
                            }
                        }
                    };


                    var resultCreateBol = srv.createBillOfLading(sessionGuid, pickingData, paramLanguage.BG);

                    var barcode = resultCreateBol.generatedParcels.First().parcelId;

                    //  _logger.InsertLog(LogLevel.Information, string.Format(@"Bill of Landing created. TrackNumber={0}", barcode));


                    shipment.BolCreatingStatus = BolCreatingStatus.BolIsCreated;
                    shipment.CourierStatus = pickingData.willBringToOffice
                        ? CourierStatus.BringToOffice
                        : CourierStatus.NotRequested;
                    shipment.BolDateCreated = DateTime.Now;
                    shipment.BarCode = barcode.ToString();

                    shipment.ShippingCost = (decimal)resultCreateBol.amounts.total - ((decimal)resultCreateBol.amounts.codPremium * 1.2m) + 1; //Add 1 bgn
                    shipment.ShippingCost = Math.Ceiling(shipment.ShippingCost * 2) / 2;  //Round to 0.5

                    shipment.CodComission = (decimal)resultCreateBol.amounts.codPremium * 1.2m;

                    _shipmentTaskRep.Update(shipment);

                    var nopShipment = new Shipment
                    {
                        OrderId = order.Id,
                        AdminComment = "0",
                        CreatedOnUtc = DateTime.Now,
                        TotalWeight = (decimal)weight,
                        TrackingNumber = barcode.ToString(),
                    };

                    orderItems.ForEach(w =>
                    {
                        nopShipment.ShipmentItems.Add(new ShipmentItem
                        {
                            OrderItemId = w.Id,
                            Quantity = w.Quantity,
                        });
                    });
                    _shipmentService.InsertShipment(nopShipment);

                }
                catch (Exception e)
                {
                    shipment.BolCreatingStatus = BolCreatingStatus.ErrorBolCreating;
                    shipment.BolCreatingErrorMessage = e.Message;
                    _shipmentTaskRep.Update(shipment);

                    _logger.Error("Error BillOfLanding creating", e);
                }
            }
        }

        public decimal CalculateShippingByCartAndBol(int serviceId, List<GetShippingOptionRequest.PackageItem> cartItems, long? officeId, long siteId)
        {
            var cartItemsSplited = cartItems.GroupBy(w => new { w.ShoppingCartItem.Product.VendorId, w.ShoppingCartItem.Product.IsFreeShipping });

            decimal result = 0;
            cartItemsSplited.Where(w => !w.Key.IsFreeShipping).ForEach(w => { result += Calculate(serviceId, w.ToList(), w.Key.VendorId, !officeId.HasValue || officeId < 0 ? 0 : officeId.Value, siteId); });

            return result;

        }


        private decimal Calculate(int serviceId, List<GetShippingOptionRequest.PackageItem> cartItems, int vendorId, long officeId, long siteId)
        {
            using (var srv = GetSpeedyClient())
            {
                var takingDate = DateTime.Today.AddDays(1).AddHours(9).AddMinutes(0);
                var allowedDaysForTaking = srv.getAllowedDaysForTaking(sessionGuid, serviceId, 0, 0, takingDate, 0, null, login.clientId);

                takingDate = allowedDaysForTaking.Min();

                try
                {
                    var amount = (double)cartItems.Sum(s => s.ShoppingCartItem.Product.Price * s.GetQuantity());
                    var weight = (double)cartItems.Sum(s => s.ShoppingCartItem.Product.Weight);
                    var payerType = 0;
                    long senderSiteId = 0;
                    long senderCountryId = 100;

                    if (vendorId > 0)
                    {
                        payerType = 2; //if vendor then payer type is third party

                        var vendor = _vendorsRep.GetById(vendorId);
                        var vendorAddress = _addressesRep.GetById(vendor.AddressId);
                        var senderSite = srv.listSitesEx(login.sessionId, new paramFilterSite
                        {
                            postCode = vendorAddress.ZipPostalCode
                        }, paramLanguage.BG).First().site;

                        senderSiteId = senderSite.id;
                        senderCountryId = senderSite.countryId;
                    }
                    else
                    {
                        var client = srv.getClientById(login.sessionId, login.clientId);

                        senderSiteId = client.address.siteId;
                        senderCountryId = client.address.countryId;
                    }

                    var calcData = new paramCalculation()
                    {
                        officeToBeCalledId = officeId,
                        officeToBeCalledIdSpecified = officeId > 0,
                        takingDate = takingDate,
                        takingDateSpecified = true,
                        serviceTypeId = serviceId,

                        broughtToOffice = false,

                        senderSiteId = senderSiteId,
                        senderSiteIdSpecified = senderSiteId > 0,

                        receiverSiteId = siteId,
                        receiverSiteIdSpecified = officeId <= 0,

                        senderCountryId = senderCountryId,
                        senderCountryIdSpecified = true,

                        parcelsCount = cartItems.Count,
                        weightDeclared = weight > 0 ? weight : _speedySettings.DefaultWeight,

                        documents = false,
                        fragile = false,
                        palletized = false,

                        payerType = payerType,
                        payerTypeInsurance = payerType,
                        payerTypeInsuranceSpecified = true,
                        payerRefId = payerType == 2 ? login.clientId : 0,
                        payerRefIdSpecified = payerType == 2,
                        payerRefInsuranceId = payerType == 2 ? login.clientId : 0,
                        payerRefInsuranceIdSpecified = _speedySettings.UseInsurance && payerType == 2,
                        payerTypePackings = payerType,
                        payerTypePackingsSpecified = true,
                        payerRefPackingsId = payerType == 2 ? login.clientId : 0,
                        payerRefPackingsIdSpecified = payerType == 2,
                        amountInsuranceBase = amount,
                        amountInsuranceBaseSpecified = _speedySettings.UseInsurance,
                        amountCodBase = amount,
                        amountCodBaseSpecified = false,
                    };

                    var resultCalculate = srv.calculate(sessionGuid, calcData);
                    var totalAmount = (decimal)resultCalculate.amounts.total + 1; //Add 1 bgn;
                    return Math.Ceiling(totalAmount * 2) / 2;  //Round to 0.5
                }
                catch (Exception e)
                {
                    _logger.Error("Error speedy calculating", e);
                    return 0;
                }
            }
        }

        public void RequestCourier(int[] shipmentIds, DateTime? date, string contactName, string phoneNumber)
        {
            var shipments = _shipmentTaskRep.Table.Where(w =>
                shipmentIds.Contains(w.Id)
                && w.BolCreatingStatus == BolCreatingStatus.BolIsCreated
                && w.CourierStatus == CourierStatus.NotRequested
                && !string.IsNullOrEmpty(w.BarCode)).ToList();

            if (shipments.Any())
            {
                var bolIds = shipments.Select(s => (long?)long.Parse(s.BarCode)).ToArray();

                using (var srv = GetSpeedyClient())
                {

                    var result = srv.createOrder(login.sessionId, new paramOrder()
                    {
                        billOfLadingsList = bolIds,
                        billOfLadingsToIncludeType = 10,
                        contactName = contactName,
                        pickupDate = date ?? DateTime.Now,
                        pickupDateSpecified = date.HasValue,
                        phoneNumber = new paramPhoneNumber()
                        {
                            number = phoneNumber
                        }
                    });

                    foreach (var res in result)
                    {
                        var bolNumber = res.billOfLading;
                        var shipment = shipments.Single(s => s.BarCode == bolNumber.ToString());

                        shipment.CourierStatus = res.errorDescriptions != null && res.errorDescriptions.Any()
                            ? CourierStatus.ErrorRequested
                            : CourierStatus.Requested;

                        _shipmentTaskRep.Update(shipment);

                        if (res.errorDescriptions != null && res.errorDescriptions.Any())
                            _logger.InsertLog(LogLevel.Warning,
                                string.Format(@"Error courier request for shipment with barCode=[{0}]", bolNumber),
                                string.Join('\n', res.errorDescriptions));

                    }
                }

            }
            else
            {
                throw new Exception("Shipments list is empty!");
            }


        }

        public List<ShipmentStatusEvent> GetTrackingEvents(string trackingNumber)
        {
            using (var srv = GetSpeedyClient())
            {
                var items = srv.trackPickingEx(sessionGuid, trackingNumber, paramLanguage.BG, false);


                return items?.Select(s => new ShipmentStatusEvent
                {
                    CountryCode = "100",
                    Date = s.moment,
                    Location = s.siteName,
                    EventName = s.operationDescription
                }).ToList() ?? new List<ShipmentStatusEvent>();
            }
        }

        public resultSite GetSiteInfoByBarCode(string barCode)
        {
            using (var srv = GetSpeedyClient())
            {
                var site = srv.listSitesEx(login.sessionId, new paramFilterSite
                {
                    postCode = barCode
                }, paramLanguage.BG).FirstOrDefault();
                if (site == null)
                    return null;

                return site.site;
            }
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


    public interface ISpeedyShipmentsService
    {
        decimal CalculateShippingByCartAndBol(int serviceId, List<GetShippingOptionRequest.PackageItem> cartItems, long? officeId, long siteId);
        void RunCheckingShipmentsList();
        void CancelBol(int shipmentId, string message);
        void RequestCourier(int[] shipmentIds, DateTime? date, string contactName, string phoneNumber);
        List<ShipmentStatusEvent> GetTrackingEvents(string trackingNumber);
        resultSite GetSiteInfoByBarCode(string barCode);
    }
}
