namespace Jobbie.Sample.Scheduler.Host.Infrastructure.WebApi.Hosting
{
    internal sealed class HostConfiguration : IHostConfiguration
    {
        public string Url => "http://*:31900";

        public override string ToString() => $"Url={Url}";
    }
}