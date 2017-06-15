using System.Collections.Generic;
using System.Linq;
using Jobbie.Sample.Scheduler.Contracts.Api;
using Jobbie.Sample.Scheduler.Host.Infrastructure.Versioning;
using WebApi.Hal;

namespace Jobbie.Sample.Scheduler.Host.Infrastructure.Hypermedia
{
    internal sealed class ApiVersionAppender : IHypermediaAppender<ApiVersion>
    {
        private readonly ILatestApiVersion _version;

        public ApiVersionAppender(
            ILatestApiVersion version)
        {
            _version = version;
        }

        public void Append(ApiVersion resource, IEnumerable<Link> configured)
        {
            var self = configured.First(l => l.Rel == "self");
            var curie = self.Curie;

            resource.Links.Add(
                self.CreateLink(new {versionNumber = resource.VersionNumber}));

            resource.Links.Add(
                curie.CreateLink<Schedule>(
                    Relationships.Schedule,
                    $"~/v{_version}/schedule/{{scheduleId}}"));

            resource.Links.Add(
                curie.CreateLink<Schedule>(
                    Relationships.Schedule_Query,
                    $"~/v{_version}/schedule?pageIndex={{pageIndex}}&pageSize={{pageSize}}&sortDirection={{sortDirection}}&sortProperty={{sortProperty}}"));

            resource.Links.Add(
                curie.CreateLink<Schedule>(
                    Relationships.Schedule_QueryBy_Description,
                    $"~/v{_version}/schedule?description={{description}}&pageIndex={{pageIndex}}&pageSize={{pageSize}}&sortDirection={{sortDirection}}&sortProperty={{sortProperty}}"));

            resource.Links.Add(
                curie.CreateLink<Job>(
                    Relationships.Job,
                    $"~/v{_version}/job/{{jobId}}"));

            resource.Links.Add(
                curie.CreateLink<Job>(
                    Relationships.Job_Create,
                    $"~/v{_version}/job"));

            resource.Links.Add(
                curie.CreateLink<Job>(
                    Relationships.Job_Query,
                    $"~/v{_version}/job?pageIndex={{pageIndex}}&pageSize={{pageSize}}&sortDirection={{sortDirection}}&sortProperty={{sortProperty}}"));

            resource.Links.Add(
                curie.CreateLink<Job>(
                    Relationships.Job_QueryBy_Description,
                    $"~/v{_version}/job?description={{description}}&pageIndex={{pageIndex}}&pageSize={{pageSize}}&sortDirection={{sortDirection}}&sortProperty={{sortProperty}}"));
        }
    }
}