using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Shipping.Speedy.Models;

namespace Nop.Plugin.Shipping.Speedy.Domain
{
    public class SpeedyShipment : BaseEntity
    {
        public DateTime DateCreated { get; set; }

        public int OrderId { get; set; }
        public int VendorId { get; set; }
        //public virtual Order Order { get; set; }

        public DeliveryOption DeliveryOption { get; set; }

        public long SiteId { get; set; }
        public long? OfficeId { get; set; }
        public long ServiceId { get; set; }

        public bool IsFreeShipping { get; set; }
        public bool UseCod { get; set; }

        public decimal ShippingCost { get; set; }
        public decimal CodComission { get; set; }


        #region Customer

        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }

        public string StreetName { get; set; }
        public string StreetNo { get; set; }
        public string QuarterName { get; set; }
        public string QuarterType { get; set; }
        public long QuarterId { get; set; }
        public string Block { get; set; }
        public string Entrance { get; set; }
        public string Floor { get; set; }
        public string ApNumber { get; set; }
        public string AddressNote { get; set; }
        public string Comment { get; set; }


        public string BarCode { get; set; }

        public DateTime? BolDateCreated { get; set; }

        public BolCreatingStatus BolCreatingStatus { get; set; }
        public CourierStatus CourierStatus { get; set; }
        public long CountryId { get; set; }
        public string BolCreatingErrorMessage { get; set; }
        

        #endregion

    }
}
