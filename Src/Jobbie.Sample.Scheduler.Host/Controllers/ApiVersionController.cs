using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Jobbie.Sample.Scheduler.Contracts.Api;
using Jobbie.Sample.Scheduler.Host.Infrastructure.Versioning;
using Jobbie.Sample.Scheduler.Host.Infrastructure.WebApi;

namespace Jobbie.Sample.Scheduler.Host.Controllers
{
    public sealed class ApiVersionController : ApiController
    {
        private readonly ISupportedApiVersions _supported;

        public ApiVersionController(
            ISupportedApiVersions supported)
        {
            _supported = supported;
        }

        public IHttpActionResult Get([FromUri] Page page)
        {
            var versions = OrderBy(_supported.Select(Convert), page).ToList();

            var pager = new Pager<ApiVersion>(() => versions, () => versions.Count, Relationships.ApiVersion_Query, Curies.Jobbie);

            return Ok(pager.GetPage(page));
        }

        public IHttpActionResult Get([FromUri] string id)
        {
            var version = _supported.FirstOrDefault(v => v.VersionNumber == id);

            if (version == null)
                return NotFound();

            return Ok(new ApiVersion(version.VersionNumber));
        }

        private static ApiVersion Convert(IApiVersion v) => new ApiVersion(v.VersionNumber);

        private static IEnumerable<ApiVersion> OrderBy(IEnumerable<ApiVersion> versions, IPage page) =>
            page.SortDirection == SortDirection.Ascending
                ? versions.OrderBy(v => v.VersionNumber)
                : versions.OrderByDescending(v => v.VersionNumber);
    }
}