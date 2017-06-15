using Jobbie.Sample.Scheduler.Contracts.Api;

namespace Jobbie.Sample.Scheduler.Host.Infrastructure.Versioning
{
    internal sealed class LatestApiVersion : ApiVersion, ILatestApiVersion
    {
        public LatestApiVersion(IApiVersion version)
            : base(version.VersionNumber)
        {

        }
    }
}