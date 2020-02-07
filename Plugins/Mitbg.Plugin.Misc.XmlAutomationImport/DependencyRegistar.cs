using Autofac;
using Autofac.Core;
using Mitbg.Plugin.Misc.XmlAutomationImport.Data;
using Mitbg.Plugin.Misc.XmlAutomationImport.Data.Entities;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Web.Framework.Infrastructure.Extensions;

namespace Mitbg.Plugin.Misc.XmlAutomationImport
{
    public class DependencyRegistar : IDependencyRegistrar
    {
        private const string CONTEXT_NAME = "nop_object_context_xml_automation_import_template";

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<XmlAutomationImportTemplateObjectContext>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();

            //override required repository with our custom context
            builder.RegisterType<EfRepository<XmlAutomationImportTemplate>>().As<IRepository<XmlAutomationImportTemplate>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();

            //data context
            builder.RegisterPluginDataContext<XmlAutomationImportTemplateObjectContext>(CONTEXT_NAME);
        }

        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order => 1;
    }
}
