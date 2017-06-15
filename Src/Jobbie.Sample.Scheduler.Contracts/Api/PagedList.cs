using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using WebApi.Hal;
using WebApi.Hal.Interfaces;

namespace Jobbie.Sample.Scheduler.Contracts.Api
{
    public class PagedList<T> : SimpleListRepresentation<T>
        where T : IResource
    {
        public int PageIndex { get; }
        public int PageSize { get; }
        public int KnownPagesAvailable { get; }
        public int TotalItemsCount { get; }
        public SortDirection SortDirection { get; }
        public string SortProperty { get; }

        public sealed override string Rel { get; set; }

        [JsonIgnore]
        public string Curie { get; }

        public PagedList(
            int pageIndex,
            int pageSize,
            int knownPagesAvailable,
            int totalItemsCount,
            IEnumerable<T> items,
            string rel,
            string curie,
            SortDirection sortDirection,
            string sortProperty)
            : base(items.ToList())
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            KnownPagesAvailable = knownPagesAvailable;
            TotalItemsCount = totalItemsCount;

            var relationship = rel.Contains(':') ? rel : $"{curie}:{rel}";
            var item = ResourceList.FirstOrDefault();
            if (item != null)
                item.Rel = relationship;
            Rel = relationship;
            Curie = curie;
            SortDirection = sortDirection;
            SortProperty = sortProperty;
        }

        protected PagedList(PagedList<T> inner)
            : this(
                inner.PageIndex,
                inner.PageSize,
                inner.KnownPagesAvailable,
                inner.TotalItemsCount,
                inner.ResourceList,
                inner.Rel,
                inner.Curie,
                inner.SortDirection,
                inner.SortProperty)
        {

        }
    }
}