using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Orders;
using Nop.Data.Mapping;
using Nop.Plugin.Shipping.Speedy.Domain;

namespace Nop.Plugin.Shipping.Speedy.Data
{
    public partial class SpeedyShipmentMap : NopEntityTypeConfiguration<SpeedyShipment>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<SpeedyShipment> builder)
        {
            builder.ToTable("SpeedyShipments");
            builder.HasKey(rate => rate.Id);

            //builder.HasOne(sh => sh.Order)
            //    .WithMany()
            //    .HasForeignKey(sh => sh.OrderId)
            //    .OnDelete(DeleteBehavior.Cascade);

        }

        #endregion
    }
}
