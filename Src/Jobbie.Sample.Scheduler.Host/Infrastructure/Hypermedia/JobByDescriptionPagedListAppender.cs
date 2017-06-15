using System.Collections.Generic;
using System.Linq;
using Jobbie.Sample.Scheduler.Contracts.Api;
using WebApi.Hal;

namespace Jobbie.Sample.Scheduler.Host.Infrastructure.Hypermedia
{
    internal sealed class JobByDescriptionPagedListAppender : IHypermediaAppender<JobByDescriptionPagedList>
    {
        public void Append(JobByDescriptionPagedList resource, IEnumerable<Link> configured)
        {
            var self = configured.First(l => l.Rel == "self");
            var curie = self.Curie;
            var href = self.Href;
            var parameters =
                new
                {
                    pageIndex = resource.PageIndex,
                    pageSize = resource.PageSize,
                    sortDirection = resource.SortDirection,
                    sortProperty = resource.SortProperty,
                    description = resource.Description
                };
            resource.Links.Add(new Link("self", href, curie).CreateLink(parameters));
        }
    }
}