using Mitbg.Plugin.Misc.XmlAutomationImport.Data.Entities;

namespace Mitbg.Plugin.Misc.XmlAutomationImport.Services
{
    public interface IXmlAutomationImportService
    {
        void Execute(XmlAutomationImportTemplate xmlAutomationImportTemplate);
    }
}
