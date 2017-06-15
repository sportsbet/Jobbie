using System.Collections.Generic;
using System.Linq;
using Jobbie.Sample.Scheduler.Contracts.Api;
using WebApi.Hal;

namespace Jobbie.Sample.Scheduler.Host.Infrastructure.Hypermedia
{
    internal sealed class ApiErrorAppender : IHypermediaAppender<ApiError>
    {
        public void Append(ApiError resource, IEnumerable<Link> configured) => resource.Links = configured.ToList();
    }
}