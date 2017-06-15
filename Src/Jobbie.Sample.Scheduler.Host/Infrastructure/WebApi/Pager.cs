using System;
using System.Collections.Generic;
using System.Linq;
using Jobbie.Sample.Scheduler.Contracts.Api;
using WebApi.Hal.Interfaces;

namespace Jobbie.Sample.Scheduler.Host.Infrastructure.WebApi
{
    internal class Pager<T> where T : IResource
    {
        private readonly Func<IEnumerable<T>> _getter;
        private readonly Func<int> _counter;
        private readonly string _rel;
        private readonly string _curie;

        public Pager(Func<IEnumerable<T>> getter, Func<int> counter, string rel, string curie)
        {
            _getter = getter;
            _counter = counter;
            _rel = rel;
            _curie = curie;
        }

        public PagedList<T> GetPage(IPage page)
        {
            var pageIndex = Page.DefaultPageIndex;
            var pageSize = Page.DefaultPageSize;
            var sortDirection = Page.DefaultSortDirection;
            var sortProperty = Page.DefaultSortProperty;

            if (page != null)
            {
                pageIndex = Math.Max(page.PageIndex, pageIndex);
                pageSize = Math.Min(page.PageSize, Page.MaxPageSize);
                pageSize = pageSize <= 0 ? Page.DefaultPageSize : pageSize;
                sortDirection = page.SortDirection;
                sortProperty = page.SortProperty;
            }

            var totalRecords = _counter();
            var totalPages = totalRecords / pageSize;
            if (totalRecords % pageSize > 0)
                totalPages += 1;

            var items =
                totalRecords == 0 || pageIndex > totalPages
                    ? Enumerable.Empty<T>()
                    : _getter().Skip(pageIndex * pageSize).Take(pageSize);
            return new PagedList<T>(pageIndex, pageSize, totalPages, totalRecords, items, _rel, _curie, sortDirection, sortProperty);
        }
    }
}