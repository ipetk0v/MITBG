using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mitbg.Plugin.Misc.ExternalExport.Services;
using Nop.Core.Domain.Tasks;
using Nop.Services.Plugins;
using Nop.Services.Tasks;

namespace Mitbg.Plugin.Misc.ExternalExport
{
    public class ExternalExportPlugin : BasePlugin, IScheduleTask
    {
        private readonly IExportManager _exportManager;
        private readonly IScheduleTaskService _scheduleTaskService;

        public ExternalExportPlugin(IExportManager exportManager, IScheduleTaskService scheduleTaskService)
        {
            _exportManager = exportManager;
            _scheduleTaskService = scheduleTaskService;
        }

        public void Execute()
        {
            _exportManager.GenerateFile();
        }

        public override void Install()
        {
            _scheduleTaskService.InsertTask(new ScheduleTask
            {
                Enabled = false,
                Type = typeof(ExternalExportPlugin).FullName,
                Name = "Creating external export file products.xml. For run manually",
                StopOnError = false,
                Seconds = 60 * 60 * 24 * 30 //30 day

            });

            _scheduleTaskService.InsertTask(new ScheduleTask
            {
                Enabled = false,
                Type = typeof(WatcherTask).FullName,
                Name = "Creating external export file products.xml. Task watcher",
                StopOnError = false,
                Seconds = 60 * 5 //5 min

            });

            base.Install();

        }

        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            var task = _scheduleTaskService.GetTaskByType(typeof(ExternalExportPlugin).FullName);
            if (task != null)
                _scheduleTaskService.DeleteTask(task);

            var taskWatcher = _scheduleTaskService.GetTaskByType(typeof(WatcherTask).FullName);
            if (taskWatcher != null)
                _scheduleTaskService.DeleteTask(taskWatcher);

        }
    }


}
