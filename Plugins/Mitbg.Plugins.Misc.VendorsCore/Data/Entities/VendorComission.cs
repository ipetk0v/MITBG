using System;
using Nop.Core;


namespace Mitbg.Plugins.Misc.VendorsCore.Domain.Entities
{
    public class VendorComission : BaseEntity
    {
        public DateTime DateCreated { get; set; }

        public int VendorId { get; set; }
        public int? CategoryId { get; set; }

        public decimal ComissionPercentage { get; set; }


    }
}
