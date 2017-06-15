using System.Web.Http.Filters;
using Jobbie.Infrastructure;

namespace Jobbie.Sample.Scheduler.Host.Infrastructure.WebApi.Filters
{
    internal sealed class HttpFilterBootstrapper : IBootstrapper
    {
        private readonly HttpFilterCollection _filters;
        private readonly ApiExceptionHandler _exceptionHandler;

        public HttpFilterBootstrapper(
            HttpFilterCollection filters,
            ApiExceptionHandler exceptionHandler)
        {
            _filters = filters;
            _exceptionHandler = exceptionHandler;
        }

        public void Init() => _filters.Add(_exceptionHandler);
    }
}