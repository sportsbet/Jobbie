using System;
using Newtonsoft.Json;

namespace Jobbie.Sample.Scheduler.Contracts.Api
{
    public sealed class ScheduleByJobPagedList : PagedList<Schedule>
    {
        public ScheduleByJobPagedList(PagedList<Schedule> inner, Guid jobId) : base(inner)
        {
            JobId = jobId;
        }

        [JsonIgnore]
        public Guid JobId { get; }
    }
}