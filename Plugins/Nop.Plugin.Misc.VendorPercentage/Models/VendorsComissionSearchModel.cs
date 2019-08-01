using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.VendorPercentage.Models
{
    public class VendorsComissionSearchModel : BaseSearchModel
    {
        #region Ctor

        public VendorsComissionSearchModel()
        {

        }

        #endregion

        #region Properties


        [NopResourceDisplayName("VendorPercentage.VendorsList.VendorName")]
        public string VendorName { get; set; }

        [NopResourceDisplayName("VendorPercentage.VendorsList.DateFrom")]
        [UIHint("DateNullable")]
        public DateTime? DateFrom { get; set; }

        [NopResourceDisplayName("VendorPercentage.VendorsList.DateTo")]
        [UIHint("DateNullable")]
        public DateTime? DateTo { get; set; }

        #endregion

    }
}
