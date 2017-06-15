using System.Collections.Generic;
using System.Linq;
using Jobbie.Sample.Scheduler.Contracts.Api;
using WebApi.Hal;
using WebApi.Hal.Interfaces;

namespace Jobbie.Sample.Scheduler.Host.Infrastructure.Hypermedia
{
    internal sealed class PagedResourceAppender<T> : IHypermediaAppender<PagedList<T>> where T : IResource
    {
        public void Append(PagedList<T> resource, IEnumerable<Link> configured)
        {
            var parameters =
                new
                {
                    pageIndex = resource.PageIndex,
                    pageSize = resource.PageSize,
                    sortDirection = resource.SortDirection,
                    sortProperty = resource.SortProperty
                };
            resource.Links = configured.Where(l => l.Rel == "self").Select(l => l.CreateLink(parameters)).ToList();
        }
    }
}