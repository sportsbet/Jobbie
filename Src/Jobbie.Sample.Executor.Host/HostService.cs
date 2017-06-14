using System;
using Common.Logging;
using Quartz;

namespace Jobbie.Sample.Executor.Host
{
    internal sealed class HostService
    {
        private static readonly ILog _log = LogManager.GetLogger<HostService>();

        private readonly IScheduler _scheduler;

        public HostService(
            IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public void Start()
        {
            try
            {
                _log.Info("Starting Executor service.");
                _scheduler.Start();
            }
            catch (Exception e)
            {
                _log.Error("Failed to start Executor service.", e);
                throw;
            }
        }

        public void Stop()
        {
            try
            {
                _log.Info("Stopping Executor service.");
                _scheduler.Shutdown(true);
            }
            catch (Exception e)
            {
                _log.Error("Failed to stop Executor service.", e);
                throw;
            }
        }
    }
}