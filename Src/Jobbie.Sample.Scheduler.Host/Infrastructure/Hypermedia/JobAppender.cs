using System.Collections.Generic;
using System.Linq;
using Jobbie.Sample.Scheduler.Contracts.Api;
using Jobbie.Sample.Scheduler.Host.Infrastructure.Versioning;
using WebApi.Hal;

namespace Jobbie.Sample.Scheduler.Host.Infrastructure.Hypermedia
{
    internal sealed class JobAppender : IHypermediaAppender<Job>
    {
        private readonly ILatestApiVersion _version;

        public JobAppender(
            ILatestApiVersion version)
        {
            _version = version;
        }

        public void Append(Job resource, IEnumerable<Link> configured)
        {
            var self = configured.First(l => l.Rel == "self");
            var curie = self.Curie;

            resource.Links.Add(
                self.CreateLink(new {jobId = resource.JobId}));

            resource.Links.Add(
                curie.CreateLink<Job>(
                    Relationships.Job_Delete,
                    $"~/v{_version}/job/{resource.JobId}"));

            resource.Links.Add(
                curie.CreateLink<Schedule>(
                    Relationships.Schedule_QueryBy_Job,
                    $"~/v{_version}/schedule?jobId={resource.JobId}&pageIndex={{pageIndex}}&pageSize={{pageSize}}&sortDirection={{sortDirection}}&sortProperty={{sortProperty}}"));

            resource.Links.Add(
                curie.CreateLink<Schedule>(
                    Relationships.Schedule_Create,
                    $"~/v{_version}/schedule?jobId={resource.JobId}"));
        }
    }
}