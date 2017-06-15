using System.Web.Routing;

namespace Jobbie.Sample.Scheduler.Host.Infrastructure.Versioning
{
    internal sealed class ApiVersionFromRequestRouteReader : IApiVersionReader
    {
        private readonly RequestContext _context;
        private const string Key = "ApiVersion";

        public ApiVersionFromRequestRouteReader(
            RequestContext context)
        {
            _context = context;
        }

        public ICurrentApiVersion Read()
        {
            if (_context?.RouteData?.Values == null || !_context.RouteData.Values.ContainsKey(Key))
                throw new ApiVersionNotPresentInRequest();

            var value = _context.RouteData.Values[Key].ToString();
            return new CurrentApiVersion(value);
        }
    }
}