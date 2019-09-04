using System;
using Nop.Plugin.Shipping.Speedy.Data;
using Nop.Services.Plugins;

namespace Mitbg.Plugin.Misc.VendorsCore
{
    public class VendorsCorePlugin : BasePlugin
    {

        private readonly VendorsCoreObjectContext _objectContext;

        public VendorsCorePlugin(VendorsCoreObjectContext objectContext)
        {
            _objectContext = objectContext;
        }

        public override void Install()
        {
            //database objects
            _objectContext.Install();
            base.Install();

        }

        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            _objectContext.Uninstall();
        }
    }
}
