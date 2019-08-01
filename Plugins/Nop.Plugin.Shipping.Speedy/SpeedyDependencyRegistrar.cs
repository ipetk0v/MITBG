using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Autofac.Core;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Shipping.Speedy.Data;
using Nop.Plugin.Shipping.Speedy.Domain;
using Nop.Plugin.Shipping.Speedy.Services;
using Nop.Web.Framework.Infrastructure.Extensions;

namespace Nop.Plugin.Shipping.Speedy
{
    public class SpeedyDependencyRegistrar : IDependencyRegistrar
    {
        private const string CONTEXT_NAME = "nop_object_context_speedy_shipment";

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<SpeedyContext>().As<ISpeedyContext>().SingleInstance();
            builder.RegisterType<SpeedyShipmentsService>().As<ISpeedyShipmentsService>().InstancePerLifetimeScope();


            builder.RegisterType<SpeedyShipmentObjectContext>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();


            //override required repository with our custom context
            builder.RegisterType<EfRepository<SpeedyShipment>>().As<IRepository<SpeedyShipment>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();


            //data context
            builder.RegisterPluginDataContext<SpeedyShipmentObjectContext>(CONTEXT_NAME);
        }

        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order => 1;
    }
}
