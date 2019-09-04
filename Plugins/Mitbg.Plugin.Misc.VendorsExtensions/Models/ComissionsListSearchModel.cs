using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Mitbg.Plugin.Misc.VendorsExtensions.Models
{
    public class ComissionsListSearchModel : BaseSearchModel
    {
        #region Ctor

        public ComissionsListSearchModel()
        {

        }

        #endregion

        #region Properties


        [NopResourceDisplayName("VendorPercentage.ComissionsList.Vendor")]
        public int VendorId { get; set; }

        [NopResourceDisplayName("VendorPercentage.ComissionsList.Category")]
        public int CategoryId { get; set; }


        #endregion

    }
}
