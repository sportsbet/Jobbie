using System;

namespace Jobbie.Domain.Models
{
    public sealed class Schedule
    {
        private Schedule(Guid scheduleId, Guid jobId, string description, DateTime createdUtc, DateTime startUtc)
        {
            ScheduleId = scheduleId;
            JobId = jobId;
            Description = description;
            CreatedUtc = createdUtc;
            StartUtc = startUtc;
        }

        public Guid ScheduleId { get; }

        public Guid JobId { get; }

        public string Description { get; }

        public DateTime CreatedUtc { get; }

        public DateTime StartUtc { get; }

        public DateTime? NextUtc { get; private set; }

        public DateTime? PreviousUtc { get; private set; }

        public string Cron { get; private set; }

        public DateTime? EndUtc { get; private set; }

        public override string ToString() => $"ScheduleId={ScheduleId}|Description={Description}";

        public static Builder Init(Guid scheduleId, Guid jobId, string description, DateTime createdUtc, DateTime startUtc) =>
            new Builder(scheduleId, jobId, description, createdUtc, startUtc);

        public sealed class Builder
        {
            internal Guid ScheduleId { get; }
            internal Guid JobId { get; }
            internal string Description { get; }
            internal DateTime CreatedUtc { get; }
            internal DateTime StartUtc { get; }
            internal DateTime? NextUtc { get; private set; }
            internal DateTime? PreviousUtc { get; private set; }
            internal string Cron { get; private set; }
            internal DateTime? EndUtc { get; private set; }

            public Builder(Guid scheduleId, Guid jobId, string description, DateTime createdUtc, DateTime startUtc)
            {
                ScheduleId = scheduleId;
                JobId = jobId;
                Description = description;
                CreatedUtc = createdUtc;
                StartUtc = startUtc;
            }

            public Builder WithNextUtc(DateTime nextUtc)
            {
                NextUtc = nextUtc;
                return this;
            }

            public Builder WithPreviousUtc(DateTime previousUtc)
            {
                PreviousUtc = previousUtc;
                return this;
            }

            public Builder WithCron(string cron)
            {
                Cron = cron;
                return this;
            }

            public Builder WithEndUtc(DateTime endUtc)
            {
                EndUtc = endUtc;
                return this;
            }

            public Schedule Build() =>
                new Schedule(ScheduleId, JobId, Description, CreatedUtc, StartUtc)
                {
                    NextUtc = NextUtc,
                    PreviousUtc = PreviousUtc,
                    Cron = Cron,
                    EndUtc = EndUtc
                };
        }
    }
}