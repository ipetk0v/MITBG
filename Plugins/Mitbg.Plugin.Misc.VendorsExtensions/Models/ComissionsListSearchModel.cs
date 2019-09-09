using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
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


        [NopResourceDisplayName("VendorComissionsEditor.ComissionsList.Filter.Vendor")]
        public int VendorId { get; set; }

        [NopResourceDisplayName("VendorComissionsEditor.ComissionsList.Filter.Category")]
        public int CategoryId { get; set; }

        public IList<SelectListItem> Vendors { get; set; }
        public IList<SelectListItem> Categories { get; set; }


        #endregion

    }
}
