using Newtonsoft.Json;

namespace Jobbie.Sample.Scheduler.Contracts.Api
{
    public sealed class ScheduleByDescriptionPagedList : PagedList<Schedule>
    {
        public ScheduleByDescriptionPagedList(PagedList<Schedule> inner, string description) : base(inner)
        {
            Description = description;
        }

        [JsonIgnore]
        public string Description { get; }
    }
}