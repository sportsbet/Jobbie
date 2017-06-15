using System.Collections.Generic;
using System.Linq;
using Jobbie.Sample.Scheduler.Contracts.Api;
using Jobbie.Sample.Scheduler.Host.Infrastructure.Versioning;
using WebApi.Hal;

namespace Jobbie.Sample.Scheduler.Host.Infrastructure.Hypermedia
{
    internal sealed class SchedulerAppender : IHypermediaAppender<Schedule>
    {
        private readonly ILatestApiVersion _version;

        public SchedulerAppender(
            ILatestApiVersion version)
        {
            _version = version;
        }

        public void Append(Schedule resource, IEnumerable<Link> configured)
        {
            var self = configured.First(l => l.Rel == "self");
            var curie = self.Curie;

            resource.Links.Add(
                self.CreateLink(new {scheduleId = resource.ScheduleId}));

            resource.Links.Add(
                curie.CreateLink<Schedule>(
                    Relationships.Schedule_Create,
                    $"~/v{_version}/schedule/{resource.ScheduleId}"));

            resource.Links.Add(
                curie.CreateLink<Schedule>(
                    Relationships.Schedule_Delete,
                    $"~/v{_version}/schedule/{resource.ScheduleId}"));

            resource.Links.Add(
                curie.CreateLink<Job>(
                    Relationships.Job,
                    $"~/v{_version}/job/{resource.JobId}"));
        }
    }
}