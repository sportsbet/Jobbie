namespace Jobbie.Sample.Scheduler.Host.Infrastructure.WebApi.Hosting
{
    internal interface IHostConfiguration
    {
        string Url { get; }

        int Port { get; }

        bool RequiresAuthorization { get; }
    }
}