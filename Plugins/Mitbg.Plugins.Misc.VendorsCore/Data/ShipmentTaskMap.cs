using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mitbg.Plugins.Misc.VendorsCore.Domain.Entities;
using Nop.Core.Domain.Orders;
using Nop.Data.Mapping;
namespace Mitbg.Plugins.Misc.VendorsCore.Data
{
    public partial class ShipmentTaskMap : NopEntityTypeConfiguration<ShipmentTask>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<ShipmentTask> builder)
        {
            builder.ToTable("Mitbg_ShipmentTasks");
            builder.HasKey(rate => rate.Id);

            //builder.HasOne(sh => sh.Order)
            //    .WithMany()
            //    .HasForeignKey(sh => sh.OrderId)
            //    .OnDelete(DeleteBehavior.Cascade);

        }

        #endregion
    }
}
