using System;
using Mitbg.Plugins.Misc.VendorsCore.Domain;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Shipping.Speedy.Models
{
    public partial class SpeedyShipmentModel : BaseNopEntityModel
    {
        public string CustomerName { get; set; }
        public int OrderId { get; set; }
        public string ShippingCostText { get; set; }
        public string CodComissionText { get; set; }
        public bool IsFreeShipping { get; set; }
        public DateTime DateCreated { get; set; }
        public string BarCode { get; set; }
        public CourierStatus CourierStatus { get; set; }
        public string CourierStatusText { get; set; }
        public BolCreatingStatus BolCreatingStatus { get; set; }
        public string BolCreatingStatusText { get; set; }
        public string ErrorMessage { get; set; }
    }
}
