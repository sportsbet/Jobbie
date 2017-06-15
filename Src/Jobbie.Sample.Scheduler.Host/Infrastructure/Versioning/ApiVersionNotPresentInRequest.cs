using System;

namespace Jobbie.Sample.Scheduler.Host.Infrastructure.Versioning
{
    internal sealed class ApiVersionNotPresentInRequest : Exception
    {
        public ApiVersionNotPresentInRequest()
            : base("Failed to find the API version in the request.")
        {

        }
    }
}