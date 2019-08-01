using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Shipping.Speedy
{
    public partial class SpeedySettings : ISettings
    {
        public string Login { get; set; }

        public string Password { get; set; }

        public double DefaultWeight { get; set; }

        public string DefaultContent { get; set; }
        public string DefaultPackage { get; set; }
        public string SenderPhoneNumber { get; set; }
        public bool UseInsurance { get; set; }
        public bool OptionsOpen { get; set; }
        public bool OptionsTest { get; set; }
    }
}
