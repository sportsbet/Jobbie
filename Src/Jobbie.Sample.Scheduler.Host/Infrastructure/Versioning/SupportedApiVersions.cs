using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Jobbie.Sample.Scheduler.Contracts.Api;

namespace Jobbie.Sample.Scheduler.Host.Infrastructure.Versioning
{
    internal sealed class SupportedApiVersions : ReadOnlyCollection<IApiVersion>, ISupportedApiVersions
    {
        public SupportedApiVersions(IList<IApiVersion> list)
            : base(list)
        {

        }

        public ILatestApiVersion Latest => new LatestApiVersion(this.Last());
    }
}