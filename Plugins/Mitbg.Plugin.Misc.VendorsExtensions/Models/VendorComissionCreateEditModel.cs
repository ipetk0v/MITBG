using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;

namespace Mitbg.Plugin.Misc.VendorsExtensions.Models
{
    public class VendorComissionCreateEditModel : BaseNopEntityModel
    {
        public int VendorId { get; set; }
        public int CategoryId { get; set; }
        public decimal ComissionPercentage { get; set; }

        public List<SelectListItem> AvailableCategories { get; set; }
        public List<SelectListItem> AvailableVendors { get; set; }

        public VendorComissionCreateEditModel()
        {
            AvailableCategories = new List<SelectListItem>();
            AvailableVendors = new List<SelectListItem>();
        }
    }
}