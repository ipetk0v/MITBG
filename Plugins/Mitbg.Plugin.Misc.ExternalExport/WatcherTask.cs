using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Mitbg.Plugin.Misc.ExternalExport.Services;
using Nop.Services.Tasks;

namespace Mitbg.Plugin.Misc.ExternalExport
{
    public class WatcherTask : IScheduleTask
    {
        private readonly IExportManager _exportManager;

        private static Timer _fileGeneratorTimer = null;

        public WatcherTask(IExportManager exportManager)
        {
            _exportManager = exportManager;
        }

        public void Execute()
        {
            _fileGeneratorTimer?.Dispose();

            var startDelay = (TimeSpan.Parse("03:00") - DateTime.Now.TimeOfDay).TotalMilliseconds;
            startDelay = startDelay > 0 ? startDelay : startDelay * -1;

            var taskPeriod = (long)(TimeSpan.Parse("12:00").TotalMilliseconds * 2);

            _fileGeneratorTimer = new Timer((object p) => { _exportManager.GenerateFile(); }, null, (long)startDelay, taskPeriod);
        }
    }
}
