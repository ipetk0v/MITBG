using Mitbg.Plugin.Misc.ExternalExport.Services;
using Nop.Core.Infrastructure;
using System;
using System.Threading;

namespace Mitbg.Plugin.Misc.ExternalExport
{
    public class WatcherTask : IStartupTask
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

        public int Order => 0;
    }
}
