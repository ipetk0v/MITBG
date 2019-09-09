using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Services.Configuration;
using Nop.Services.Orders;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Shipping.Speedy.Controllers
{
    [ViewComponent(Name = "WidgetsSpeedy")]
    public class WidgetsSpeedyViewComponent : NopViewComponent
    {

        private readonly ISettingService _settingService;


        public WidgetsSpeedyViewComponent(ISettingService settingService)
        {
            _settingService = settingService;
        }

        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            var enabledOpc = false;
            var enabledOpcSetting = _settingService.GetSetting("ordersettings.onepagecheckoutenabled");
            if (enabledOpcSetting != null)
            {
                enabledOpc = enabledOpcSetting.Value.Equals("true", StringComparison.CurrentCultureIgnoreCase);
            }
            return View("~/Plugins/Mitbg.Plugin.Shipping.Speedy/Views/PublicInfo.cshtml", enabledOpc);
        }

    }
}
