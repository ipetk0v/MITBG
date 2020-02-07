using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mitbg.Plugin.Misc.XmlAutomationImport.Data.Entities;
using Nop.Data.Mapping;

namespace Mitbg.Plugin.Misc.XmlAutomationImport.Data
{
    public class XmlAutomationImportTemplateMap : NopEntityTypeConfiguration<XmlAutomationImportTemplate>
    {
        public override void Configure(EntityTypeBuilder<XmlAutomationImportTemplate> builder)
        {
            builder.ToTable("Mitbg_XmlAutomationImportTemplate");
            builder.HasKey(xml => xml.Id);
        }
    }
}
