using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;

namespace Mitbg.Plugin.Misc.XmlAutomationImport.Models
{
    public class XmlAutomationImportSearchModel : BaseSearchModel
    {
        public XmlAutomationImportSearchModel()
        {
            Vendors = new List<SelectListItem>();
        }

        [NopResourceDisplayName("XmlAutomationImport.Filter.Vendor")]
        public int VendorId { get; set; }

        [NopResourceDisplayName("XmlAutomationImport.Search.Vendors")]
        public IList<SelectListItem> Vendors { get; set; }
    }
}
