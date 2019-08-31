using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;
using Nop.Plugin.Shipping.Speedy.Domain;

namespace Nop.Plugin.Shipping.Speedy.Models
{
    public class ConfigurationModel
    {
        [NopResourceDisplayName("Speedy.Admin.ConfigurationFields.Login")]
        public string Login { get; set; }

        [NopResourceDisplayName("Speedy.Admin.ConfigurationFields.Password")]
        public string Password { get; set; }

        [NopResourceDisplayName("Speedy.Admin.ConfigurationFields.DefaultWeight")]
        public double DefaultWeight { get; set; }

        [NopResourceDisplayName("Speedy.Admin.ConfigurationFields.DefaultContent")]
        public string DefaultContent { get; set; }

        [NopResourceDisplayName("Speedy.Admin.ConfigurationFields.DefaultPackage")]
        public string DefaultPackage { get; set; }

        [NopResourceDisplayName("Speedy.Admin.ConfigurationFields.SenderPhoneNumber")]
        public string SenderPhoneNumber { get; set; }

        [NopResourceDisplayName("Speedy.Admin.ConfigurationFields.UseInsurance")]
        public bool UseInsurance { get; set; }

        [NopResourceDisplayName("Speedy.Admin.ConfigurationFields.OptionsOpen")]
        public bool OptionsOpen { get; set; }

        [NopResourceDisplayName("Speedy.Admin.ConfigurationFields.OptionsTest")]
        public bool OptionsTest { get; set; }

        [NopResourceDisplayName("Speedy.Admin.ConfigurationFields.CodMethod")]
        public CodMethod CodMethod { get; set; }
    }
}
