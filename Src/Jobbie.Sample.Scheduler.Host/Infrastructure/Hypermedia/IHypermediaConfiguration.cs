using WebApi.Hal;

namespace Jobbie.Sample.Scheduler.Host.Infrastructure.Hypermedia
{
    internal interface IHypermediaConfiguration
    {
        IHypermediaResolver Resolver { get; }
    }
}