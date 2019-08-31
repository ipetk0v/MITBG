using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Shipping.Speedy.Domain;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Shipping.Speedy.Models
{
    public partial class SpeedyShipmentsSearchModel : BaseSearchModel
    {
        #region Ctor

        public SpeedyShipmentsSearchModel()
        {

        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Speedy.ShipmentsList.BarCode")]
        public long? BarCode { get; set; }

        [NopResourceDisplayName("Speedy.ShipmentsList.OrderId")]
        public int? OrderId { get; set; }

        [NopResourceDisplayName("Speedy.ShipmentsList.CustomerName")]
        public string CustomerName { get; set; }


        [NopResourceDisplayName("Speedy.ShipmentsList.DateCreated")]
        [UIHint("DateNullable")]
        public DateTime? DateCreated { get; set; }

        #endregion
    }
}
