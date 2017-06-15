using System;
using Common.Logging;
using Jobbie.Sample.Scheduler.Host.Infrastructure.WebApi.Hosting;
using Microsoft.Owin.Hosting;
using Quartz;

namespace Jobbie.Sample.Scheduler.Host
{
    internal sealed class HostService
    {
        private readonly IScheduler _scheduler;
        private static readonly ILog _log = LogManager.GetLogger<HostService>();

        private readonly string _url;
        private IDisposable _host;

        public HostService(
            IHostConfiguration config,
            IScheduler scheduler)
        {
            _scheduler = scheduler;
            _url = config.Url;
        }

        public void Start()
        {
            try
            {
                _log.Info($"Starting WebApi host on {_url}.");
                _host = WebApp.Start<OwinConfiguration>(_url);
            }
            catch (Exception e)
            {
                _log.Error("Failed to start the WebApi host.", e);
                throw;
            }
        }

        public void Stop()
        {
            try
            {
                _log.Info("Stopping WebApi host.");
                _host.Dispose();
                _scheduler.Shutdown(true);
            }
            catch (Exception e)
            {
                _log.Error("Failed to stop the WebApi host.", e);
                throw;
            }
        }
    }
}