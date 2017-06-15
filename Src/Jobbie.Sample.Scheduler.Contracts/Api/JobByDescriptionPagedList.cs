using Newtonsoft.Json;

namespace Jobbie.Sample.Scheduler.Contracts.Api
{
    public sealed class JobByDescriptionPagedList : PagedList<Job>
    {
        public JobByDescriptionPagedList(PagedList<Job> inner, string description) : base(inner)
        {
            Description = description;
        }

        [JsonIgnore]
        public string Description { get; }
    }
}