namespace Jobbie.Sample.Scheduler.Contracts.Api
{
    public interface IPage
    {
        int PageIndex { get; }

        int PageSize { get; }

        SortDirection SortDirection { get; }

        string SortProperty { get; }
    }
}