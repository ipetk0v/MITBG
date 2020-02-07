using System;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Mitbg.Plugin.Misc.XmlAutomationImport.Models
{
    public class XmlAutomationImportModel : BaseNopEntityModel
    {
        public int VendorId { get; set; }

        //[NopResourceDisplayName("XmlAutomationImport.Search.Vendor")]
        public string Vendor { get; set; }

        public bool IsActive { get; set; }

        public string XmlLinkUrl { get; set; }

        public string Prefix { get; set; }

        public DateTime LastActivity { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
