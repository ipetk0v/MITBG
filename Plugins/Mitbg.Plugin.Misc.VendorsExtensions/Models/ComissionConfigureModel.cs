using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;

namespace Mitbg.Plugin.Misc.VendorsExtensions.Models
{
    public class ComissionConfigureModel : BaseNopModel

    {
        public int Id { get; set; }
        public string VendorName { get; set; }
        public int VendorId { get; set; }
        public string CategoryName { get; set; }
        public int? CategoryId { get; set; }
        public decimal Comission { get; set; }
        public DateTime DateCreate { get; set; }


       
    }
}
