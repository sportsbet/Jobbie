using System.Collections.Generic;
using Jobbie.Sample.Scheduler.Contracts.Api;

namespace Jobbie.Sample.Scheduler.Host.Infrastructure.Versioning
{
    public interface ISupportedApiVersions : IEnumerable<IApiVersion>
    {
        ILatestApiVersion Latest { get; }
    }
}