using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;
using Nop.Plugin.Shipping.Speedy.Domain;
using Nop.Plugin.Shipping.Speedy.Models;
using Nop.Services.Common;
using Nop.Services.Events;
using Nop.Services.Logging;

namespace Nop.Plugin.Shipping.Speedy
{
    public partial class OrdersConsumer : IConsumer<OrderPlacedEvent>
    {
        private readonly ISpeedyContext _speedyContext;
        private readonly ILogger _logger;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWorkContext _workContext;

        private readonly IRepository<SpeedyShipment> _speedyShipmentsRep;

        public OrdersConsumer(ISpeedyContext speedyContext, ILogger logger, IRepository<SpeedyShipment> speedyShipmentsRep, IGenericAttributeService genericAttributeService, IWorkContext workContext)
        {
            _speedyContext = speedyContext;
            _logger = logger;
            _speedyShipmentsRep = speedyShipmentsRep;
            _genericAttributeService = genericAttributeService;
            _workContext = workContext;
        }

        public async void HandleEvent(OrderPlacedEvent eventMessage)
        {
            var customer = eventMessage.Order.Customer;
            var order = eventMessage.Order;
            try
            {
                if (!string.IsNullOrEmpty(order.ShippingRateComputationMethodSystemName) && order.ShippingRateComputationMethodSystemName.Equals("Shipping.Speedy"))
                {
                    _logger.InsertLog(LogLevel.Debug, "Adding speedy shipment item");

                    var bol = _speedyContext.GetBillInfo(customer.Id);
                    if (bol != null)
                    {
                        var paymentMethod = _genericAttributeService
                            .GetAttribute<PaymentType>(_workContext.CurrentCustomer, SpeedyDefaults.CurrentPaymentTypeAttributeKey);

                        var useCod = paymentMethod == PaymentType.CashOnDelivery ||
                                 !string.IsNullOrEmpty(order.PaymentMethodSystemName) && order.PaymentMethodSystemName.Equals("Payments.CashOnDelivery",
                                     StringComparison.CurrentCultureIgnoreCase);

                        var customerName = string.Format(@"{0} {1}", order.ShippingAddress.FirstName,
                            order.ShippingAddress.LastName);
                        var customerPhone = order.ShippingAddress.PhoneNumber;

                        foreach (var vendorItems in order.OrderItems.GroupBy(w => new { w.Product.VendorId, w.Product.IsFreeShipping }))
                        {
                            var shipmentData = new SpeedyShipment
                            {
                                SiteId = bol.SiteId,
                                DeliveryOption = bol.DeliveryOption,
                                OfficeId = bol.OfficeId,
                                OrderId = order.Id,
                                VendorId = vendorItems.Key.VendorId,
                                DateCreated = DateTime.Now,
                                AddressNote = bol.AddressNote,
                                ApNumber = bol.ApNumber,
                                Block = bol.Block,
                                Comment = bol.Comment,
                                QuarterName = bol.QuarterName,
                                QuarterType = bol.QuarterName,
                                QuarterId = bol.QuarterId,
                                Entrance = bol.Entrance,
                                Floor = bol.Floor,
                                StreetName = bol.StreetName,
                                StreetNo = bol.StreetNo,
                                ServiceId = bol.ServiceId,
                                UseCod = useCod,
                                IsFreeShipping = vendorItems.Key.IsFreeShipping,
                                CustomerName = customerName,
                                CustomerPhone = customerPhone,
                                BolCreatingStatus = BolCreatingStatus.Pending
                            };
                            _speedyShipmentsRep.Insert(shipmentData);

                            //Delete speedy generic attribute
                            var keyGroup = _workContext.CurrentCustomer.GetUnproxiedEntityType().Name;
                            var attrubutes = _genericAttributeService.GetAttributesForEntity(_workContext.CurrentCustomer.Id, keyGroup);
                            var bolInfoAttribute = attrubutes.FirstOrDefault(w =>
                                w.Key == SpeedyDefaults.SpeedyShipmentConfiguresAttribute);
                            if (bolInfoAttribute != null)
                                _genericAttributeService.DeleteAttribute(bolInfoAttribute);


                            _logger.InsertLog(LogLevel.Information, "Shipment created",
                                JsonConvert.SerializeObject(shipmentData, Formatting.Indented));
                        }

                    }

                }
                else
                    throw new Exception("Bol info not founded");
            }
            catch (Exception e)
            {
                _logger.InsertLog(LogLevel.Error, "Error where creating BOL", e.ToString());

            }

        }
    }
}
