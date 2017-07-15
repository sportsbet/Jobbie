namespace Jobbie.Sample.Scheduler.Host.Infrastructure.WebApi.Hosting
{
    internal abstract class HostConfiguration : IHostConfiguration
    {
        public string Url => $"http://*:{Port}";

        public bool RequiresAuthorization { get; }

        public override string ToString() => $"Url={Url}";

        public int Port { get; }

        protected HostConfiguration(int port, bool requiresAuthorization)
        {
            Port = port;
            RequiresAuthorization = requiresAuthorization;
        }
    }

    internal sealed class PublicHostConfiguration : HostConfiguration
    {
        public PublicHostConfiguration()
            : base(31900, true)
        {
            
        }
    }

    internal sealed class InternalHostConfiguration : HostConfiguration
    {
        public InternalHostConfiguration()
            : base(31903, false)
        {
            
        }
    }
}