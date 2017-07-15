using System;
using System.Collections.Generic;
using System.Linq;
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

        private readonly IEnumerable<string> _urls;
        private IDisposable _host;

        public HostService(
            IEnumerable<IHostConfiguration> hosts,
            IScheduler scheduler)
        {
            _scheduler = scheduler;
            _urls = hosts.Select(c => c.Url);
        }

        public void Start()
        {
            try
            {
                var options = new StartOptions();
                foreach (var url in _urls)
                {
                    _log.Info($"Starting WebApi host on {url}.");
                    options.Urls.Add(url);
                }
                _host = WebApp.Start<OwinConfiguration>(options);
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