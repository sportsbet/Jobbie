using WebApi.Hal;

namespace Jobbie.Sample.Scheduler.Contracts.Api
{
    public class ApiVersion : Representation, IApiVersion
    {
        public ApiVersion(string versionNumber)
        {
            VersionNumber = versionNumber;
        }

        public string VersionNumber { get; }

        public override string ToString() => VersionNumber;
    }
}