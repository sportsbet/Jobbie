using System.Web.Http.Filters;
using Jobbie.Infrastructure;

namespace Jobbie.Sample.Scheduler.Host.Infrastructure.WebApi.Filters
{
    internal sealed class HttpFilterBootstrapper : IBootstrapper
    {
        private readonly HttpFilterCollection _filters;
        private readonly ApiExceptionHandler _exceptionHandler;
        private readonly IAuthorizationFilter _authorize;

        public HttpFilterBootstrapper(
            HttpFilterCollection filters,
            ApiExceptionHandler exceptionHandler,
            IAuthorizationFilter authorize)
        {
            _filters = filters;
            _exceptionHandler = exceptionHandler;
            _authorize = authorize;
        }

        public void Init()
        {
            _filters.Add(_exceptionHandler);
            _filters.Add(_authorize);
        }
    }
}