using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using Jobbie.Sample.Scheduler.Host.Infrastructure.WebApi.Hosting;

namespace Jobbie.Sample.Scheduler.Host.Infrastructure.WebApi.Filters
{
    internal sealed class WebApiAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly IEnumerable<IHostConfiguration> _hosts;

        public WebApiAuthorizeAttribute(
            IEnumerable<IHostConfiguration> hosts)
        {
            _hosts = hosts;
        }

        protected override bool IsAuthorized(HttpActionContext context)
        {
            var host = _hosts.FirstOrDefault(h => h.Port == context.Request.RequestUri.Port);
            if (host == null)
                throw new Exception($"Failed to find host configuration for port {context.Request.RequestUri.Port}");

            if (!host.RequiresAuthorization)
                return true;

            var principal = context.RequestContext.Principal;
            return principal != null && principal.Identity.IsAuthenticated;
        }
    }
}