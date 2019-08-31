using Autofac;
using Autofac.Core;
using Mitbg.Plugins.Misc.VendorsCore;
using Mitbg.Plugins.Misc.VendorsCore.Domain.Entities;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Shipping.Speedy.Data;
using Nop.Web.Framework.Infrastructure.Extensions;

namespace Mitbg.Plugin.Misc.VendorsCore
{
    public class DependencyRegistar : IDependencyRegistrar
    {
        private const string CONTEXT_NAME = "nop_object_context_vendors_core";

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<ShipmentContext>().As<IShipmentContext>().SingleInstance();

            builder.RegisterType<VendorsCoreObjectContext>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();


            //override required repository with our custom context
            builder.RegisterType<EfRepository<VendorComission>>().As<IRepository<VendorComission>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();

            //override required repository with our custom context
            builder.RegisterType<EfRepository<ShipmentTask>>().As<IRepository<ShipmentTask>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();


            //data context
            builder.RegisterPluginDataContext<VendorsCoreObjectContext>(CONTEXT_NAME);
        }

        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order => 1;
    }
}
