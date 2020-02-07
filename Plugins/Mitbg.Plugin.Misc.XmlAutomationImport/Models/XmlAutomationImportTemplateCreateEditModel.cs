using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using Nop.Web.Framework.Models;

namespace Mitbg.Plugin.Misc.XmlAutomationImport.Models
{
    public class XmlAutomationImportTemplateCreateEditModel : BaseNopModel
    {
        public XmlAutomationImportTemplateCreateEditModel()
        {
            AvailableVendors = new List<SelectListItem>();
        }

        public int Id { get; set; }

        public int VendorId { get; set; }

        public bool IsActive { get; set; }

        public string XmlLink { get; set; }

        public string Prefix { get; set; }

        public List<SelectListItem> AvailableVendors { get; set; }

        // product

            public int ProductForImport { get; set; }

        public string ProductTemplate { get; set; }

        public string Name { get; set; }

        public string ShortDescription { get; set; }

        public string FullDescription { get; set; }

        public string Category { get; set; }

        public string Manufacturer { get; set; }

        public string Picture { get; set; }

        public string Sku { get; set; }

        public string ManufacturerPartNumber { get; set; }

        public string StockQuantity { get; set; }

        public string Price { get; set; }

        public string OldPrice { get; set; }

        public string Weight { get; set; }
    }
}
