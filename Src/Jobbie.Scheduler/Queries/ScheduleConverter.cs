using System;
using Jobbie.Domain.Models;
using Quartz;

namespace Jobbie.Scheduler.Queries
{
    internal sealed class ScheduleConverter : IScheduleConverter
    {
        public Schedule For(ITrigger trigger)
        {
            var scheduleId = new Guid(trigger.Key.Name);
            var jobId = new Guid(trigger.JobKey.Name);
            var description = trigger.Description;
            var createdUtc = new DateTime(trigger.JobDataMap.GetLong("CreatedUtc"));
            var startUtc = trigger.StartTimeUtc.DateTime;
            var cron = trigger.JobDataMap.ContainsKey("Cron") ? trigger.JobDataMap.GetString("Cron") : null;
            var nextUtc = trigger.GetNextFireTimeUtc()?.DateTime;
            var previousUtc = trigger.GetPreviousFireTimeUtc()?.DateTime;
            var endUtc = trigger.EndTimeUtc?.DateTime;

            var schedule = Schedule.Init(scheduleId, jobId, description, createdUtc, startUtc);

            if (!string.IsNullOrEmpty(cron))
                schedule.WithCron(cron);
            if (nextUtc.HasValue)
                schedule.WithNextUtc(nextUtc.Value);
            if (previousUtc.HasValue)
                schedule.WithPreviousUtc(previousUtc.Value);
            if (endUtc.HasValue)
                schedule.WithEndUtc(endUtc.Value);

            return schedule.Build();
        }
    }
}