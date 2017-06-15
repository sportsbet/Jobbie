using System;
using WebApi.Hal;

namespace Jobbie.Sample.Scheduler.Contracts.Api
{
    public sealed class Schedule : Representation
    {
        public Guid ScheduleId { get; set; }

        public Guid JobId { get; set; }

        public string Description { get; set; }

        public DateTime CreatedUtc { get; set; }

        public DateTime StartUtc { get; set; }

        public DateTime? NextUtc { get; set; }

        public DateTime? PreviousUtc { get; set; }

        public string Cron { get; set; }

        public DateTime? EndUtc { get; set; }

        public override string ToString() => $"ScheduleId={ScheduleId}";
    }
}