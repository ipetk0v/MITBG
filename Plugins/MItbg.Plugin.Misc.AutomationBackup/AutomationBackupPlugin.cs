using System;
using System.Linq;
using Nop.Core.Domain.Tasks;
using Nop.Core.Infrastructure;
using Nop.Services.Common;
using Nop.Services.Logging;
using Nop.Services.Plugins;
using Nop.Services.Tasks;

namespace MItbg.Plugin.Misc.AutomationBackup
{
    public class AutomationBackupPlugin : BasePlugin, IScheduleTask
    {
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly IMaintenanceService _maintenanceService;
        private readonly INopFileProvider _fileProvider;
        private readonly ILogger _logger;

        public AutomationBackupPlugin(
            IScheduleTaskService scheduleTaskService,
            IMaintenanceService maintenanceService,
            INopFileProvider fileProvider,
            ILogger logger
            )
        {
            _scheduleTaskService = scheduleTaskService;
            _maintenanceService = maintenanceService;
            _fileProvider = fileProvider;
            _logger = logger;
        }

        public void Execute()
        {
            var allBackupFiles = _maintenanceService.GetAllBackupFiles();
            try
            {
                if (allBackupFiles.Count > 10)
                {
                    var backupPath = _maintenanceService.GetAllBackupFiles().Last();
                    _fileProvider.DeleteFile(backupPath);
                }

                _maintenanceService.BackupDatabase();
                _logger.Information("Successfully automation backup database!");
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                _logger.Error("Something wrong with automation backup database!");
            }
        }

        public override void Install()
        {
            _scheduleTaskService.InsertTask(new ScheduleTask
            {
                Enabled = true,
                Type = typeof(AutomationBackupPlugin).FullName,
                Name = "Creating automation backup every day",
                StopOnError = false,
                Seconds = 60 * 60 * 24 // 1 day
            });

            base.Install();

        }

        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            var task = _scheduleTaskService.GetTaskByType(typeof(AutomationBackupPlugin).FullName);
            if (task != null)
                _scheduleTaskService.DeleteTask(task);

            base.Uninstall();
        }
    }
}
