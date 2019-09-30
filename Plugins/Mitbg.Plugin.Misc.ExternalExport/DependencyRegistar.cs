using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Mitbg.Plugin.Misc.ExternalExport.Services;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;

namespace Mitbg.Plugin.Misc.ExternalExport
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {

            builder.RegisterType<ExportManager>().As<IExportManager>().InstancePerLifetimeScope();
        }

        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order => 1;
    }
}
