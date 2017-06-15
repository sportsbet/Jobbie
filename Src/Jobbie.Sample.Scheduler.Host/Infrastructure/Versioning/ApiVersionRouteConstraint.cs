using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Routing;

namespace Jobbie.Sample.Scheduler.Host.Infrastructure.Versioning
{
    internal sealed class ApiVersionRouteConstraint : IHttpRouteConstraint
    {
        private readonly ISupportedApiVersions _supported;

        public ApiVersionRouteConstraint(
            ISupportedApiVersions supported)
        {
            _supported = supported;
        }

        public bool Match(
            HttpRequestMessage request,
            IHttpRoute route,
            string parameterName,
            IDictionary<string, object> values,
            HttpRouteDirection direction)
        {
            if (!values.ContainsKey(parameterName))
                return false;

            var value = values[parameterName].ToString();
            return _supported.Any(v => v.VersionNumber == value);
        }
    }
}