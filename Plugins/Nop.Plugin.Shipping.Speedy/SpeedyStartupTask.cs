using System.ComponentModel;
using Nop.Core.Domain.Shipping;
using Nop.Core.Infrastructure;
using Nop.Plugin.Shipping.Speedy.Domain;
using Nop.Plugin.Shipping.Speedy.Models;

namespace Nop.Plugin.Shipping.Speedy
{
    public class SpeedyStartupTask : IStartupTask
    {
        public void Execute()
        {
            TypeDescriptor.AddAttributes(typeof(SpeedyBolInfo), new TypeConverterAttribute(typeof(SpeedyBolInfoTypeConverter)));
        }

        public int Order => 2;
    }
}