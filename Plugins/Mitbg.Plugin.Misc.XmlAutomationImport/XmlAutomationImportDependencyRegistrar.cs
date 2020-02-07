using Autofac;
using Mitbg.Plugin.Misc.XmlAutomationImport.Services;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;

namespace Mitbg.Plugin.Misc.XmlAutomationImport
{
    public class XmlAutomationImportDependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {

            builder.RegisterType<XmlAutomationImportService>().As<IXmlAutomationImportService>().InstancePerLifetimeScope();
        }

        public int Order => 1;
    }
}
