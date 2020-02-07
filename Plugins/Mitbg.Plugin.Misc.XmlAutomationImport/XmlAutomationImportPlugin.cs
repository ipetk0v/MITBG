using System;
using System.Linq;
using Microsoft.AspNetCore.Routing;
using Mitbg.Plugin.Misc.XmlAutomationImport.Data;
using Mitbg.Plugin.Misc.XmlAutomationImport.Data.Entities;
using Mitbg.Plugin.Misc.XmlAutomationImport.Services;
using Nop.Core.Data;
using Nop.Core.Domain.Tasks;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Services.Tasks;
using Nop.Web.Framework.Menu;

namespace Mitbg.Plugin.Misc.XmlAutomationImport
{
    public class XmlAutomationImportPlugin : BasePlugin, IAdminMenuPlugin, IScheduleTask
    {
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly IXmlAutomationImportService _xmlAutomationImportService;
        private readonly ILogger _logger;
        private readonly IRepository<XmlAutomationImportTemplate> _xmlAutomationImportTemplateRep;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly XmlAutomationImportTemplateObjectContext _objectContext;

        public XmlAutomationImportPlugin(
            IPermissionService permissionService,
            ILocalizationService localizationService,
            IXmlAutomationImportService xmlAutomationImportService,
            ILogger logger,
            IScheduleTaskService scheduleTaskService,
            IRepository<XmlAutomationImportTemplate> xmlAutomationImportTemplateRep,
            XmlAutomationImportTemplateObjectContext objectContext)
        {
            _permissionService = permissionService;
            _objectContext = objectContext;
            _xmlAutomationImportService = xmlAutomationImportService;
            _logger = logger;
            _xmlAutomationImportTemplateRep = xmlAutomationImportTemplateRep;
            _localizationService = localizationService;
            _scheduleTaskService = scheduleTaskService;
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var catalogMenu = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Catalog");

            if (catalogMenu != null)
            {
                var subMenuItem = new SiteMapNode()
                {
                    SystemName = "XmlAutomationImport",
                    Title = "Импортиране на продукти",
                    ControllerName = "XmlAutomationImport",
                    ActionName = "TemplateList",
                    IconClass = "fa-dot-circle-o",
                    Visible = true,
                    RouteValues = new RouteValueDictionary() { { "area", "Admin" } }
                };

                catalogMenu.ChildNodes.Add(subMenuItem);
            }
        }

        public void Execute()
        {
            try
            {
                var activeXmlTemplates = _xmlAutomationImportTemplateRep.Table.Where(x => x.IsActive);

                foreach (var template in activeXmlTemplates)
                {
                    _xmlAutomationImportService.Execute(template);
                }

                _logger.Information("Successfully end xml automation product import!");
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                _logger.Error("Something wrong with xml automation product import!");
            }
        }

        public override void Install()
        {
            _localizationService.AddOrUpdatePluginLocaleResource("XmlAutomationImportTemplate.Editor.AddNew", "Добавяне на нов темплейт");
            _localizationService.AddOrUpdatePluginLocaleResource("XmlAutomationImportTemplate.Editor.BackToList", "обратно към списъка с темплейти");
            _localizationService.AddOrUpdatePluginLocaleResource("XmlAutomationImportTemplate.Editor.Edit", "Редактиране на темплейт");
            _localizationService.AddOrUpdatePluginLocaleResource("XmlAutomationImport.Editor.Execute", "Изпълни трансфер");
            _localizationService.AddOrUpdatePluginLocaleResource("XmlAutomationImportTemplate.Title", "Импортиране на продукти");

            _scheduleTaskService.InsertTask(new ScheduleTask
            {
                Enabled = true,
                Type = typeof(XmlAutomationImportPlugin).FullName,
                Name = "Automation Product Xml Import",
                StopOnError = false,
                Seconds = 60 * 60 * 24 // 1 day
            });

            _objectContext.Install();
            _permissionService.InstallPermissions(new Permission());

            base.Install();
        }

        public override void Uninstall()
        {
            _localizationService.DeletePluginLocaleResource("XmlAutomationImportTemplate.Editor.AddNew");
            _localizationService.DeletePluginLocaleResource("XmlAutomationImportTemplate.Editor.BackToList");
            _localizationService.DeletePluginLocaleResource("XmlAutomationImportTemplate.Editor.Edit");
            _localizationService.DeletePluginLocaleResource("XmlAutomationImport.Editor.Execute");
            _localizationService.DeletePluginLocaleResource("XmlAutomationImportTemplate.Title");

            var task = _scheduleTaskService.GetTaskByType(typeof(XmlAutomationImportPlugin).FullName);
            if (task != null)
                _scheduleTaskService.DeleteTask(task);

            _objectContext.Uninstall();
            _permissionService.UninstallPermissions(new Permission());

            base.Uninstall();
        }
    }
}
