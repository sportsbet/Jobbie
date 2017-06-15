using System;
using Jobbie.Sample.Client.WebApi.Host.Infrastructure.WebApi.Hosting;
using Microsoft.Owin.Hosting;

namespace Jobbie.Sample.Client.WebApi.Host
{
    internal sealed class HostService
    {
        private IDisposable _host;

        public void Start()
        {
            try
            {
                _host = WebApp.Start<OwinConfiguration>("http://*:31901");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void Stop()
        {
            try
            {
                _host.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}