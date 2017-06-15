namespace Jobbie.Sample.Scheduler.Contracts.Api
{
    public sealed class Page : IPage
    {
        public const int DefaultPageIndex = 0;
        public const int DefaultPageSize = 10;
        public const int MaxPageSize = 500;
        public const SortDirection DefaultSortDirection = SortDirection.Ascending;
        public const string DefaultSortProperty = "CreatedUtc";

        public Page(int index, int size, SortDirection direction, string property)
        {
            PageIndex = index;
            PageSize = size;
            SortDirection = direction;
            SortProperty = property;
        }

        public Page()
            : this(DefaultPageIndex, DefaultPageSize, DefaultSortDirection, DefaultSortProperty)
        {

        }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public SortDirection SortDirection { get; set; }

        public string SortProperty { get; set; }

        public override string ToString() => $"Index={PageIndex}, Size={PageSize}, Sort={SortDirection}|{SortProperty}";

        public static Page Default => new Page();

        public static Page Max => new Page(DefaultPageIndex, MaxPageSize, DefaultSortDirection, DefaultSortProperty);
    }
}