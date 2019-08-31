using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Shipping.Speedy.Models
{
    public static partial class SpeedyDefaults
    {
        #region Customer attributes

        public static string SpeedyShipmentConfiguresAttribute => "SpeedyShipmentConfigures";
        public static string CurrentPaymentTypeAttributeKey = "Customer.CurrentSelectedPaymentType";

        #endregion
    }
}
