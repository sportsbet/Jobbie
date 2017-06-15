using Jobbie.Sample.Scheduler.Contracts.Api;

namespace Jobbie.Sample.Scheduler.Host.Infrastructure.Versioning
{
    internal sealed class CurrentApiVersion : ApiVersion, ICurrentApiVersion
    {
        public CurrentApiVersion(string versionNumber)
            : base(versionNumber)
        {

        }
    }
}