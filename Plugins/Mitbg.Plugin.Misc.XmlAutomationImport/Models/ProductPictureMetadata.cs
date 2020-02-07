using Nop.Core.Domain.Catalog;

namespace Mitbg.Plugin.Misc.XmlAutomationImport.Models
{
    public class ProductPictureMetadata
    {
        public Product ProductItem { get; set; }

        public string Picture1Path { get; set; }

        public string Picture2Path { get; set; }

        public string Picture3Path { get; set; }

        public bool IsNew { get; set; }
    }
}
