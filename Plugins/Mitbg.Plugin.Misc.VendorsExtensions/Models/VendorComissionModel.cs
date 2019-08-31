using System;
using System.Collections.Generic;
using System.Text;
using Nop.Web.Framework.Models;

namespace Mitbg.Plugin.Misc.VendorsExtensions.Models
{
    public class VendorComissionModel : BaseNopModel
    {

        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public decimal TotalSum { get; set; }
        public decimal TotalShippingSum { get; set; }
        public decimal FreeShippingSum { get; set; }

        public decimal Comission { get; set; }
        public decimal Transaction => TotalSum - Comission - FreeShippingSum;

        public string TotalSumText { get; set; }
        public string ComissionText { get; set; }
        public string TransactionText { get; set; }
        public string FreeShippingSumText { get; set; }
        public string CodComissionSumText { get; set; }
    }
}
