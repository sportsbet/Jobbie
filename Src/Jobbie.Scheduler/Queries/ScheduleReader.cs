using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Jobbie.Domain.Models;
using Jobbie.Domain.Queries;
using Quartz;
using Quartz.Impl.Matchers;

namespace Jobbie.Scheduler.Queries
{
    internal sealed class ScheduleReader : IScheduleReader
    {
        private readonly IScheduler _scheduler;
        private readonly IScheduleConverter _converter;

        public ScheduleReader(
            IScheduler scheduler,
            IScheduleConverter converter)
        {
            _scheduler = scheduler;
            _converter = converter;
        }

        public Schedule For(Guid scheduleId)
        {
            var trigger = _scheduler.GetTrigger(new TriggerKey(scheduleId.ToString()));
            return trigger == null ? null : Convert(trigger);
        }

        public IReadOnlyCollection<Schedule> All() =>
            _scheduler
                .GetTriggerKeys(GroupMatcher<TriggerKey>.AnyGroup())
                .Select(k => _scheduler.GetTrigger(k))
                .Select(Convert)
                .ToList();

        public IReadOnlyCollection<Schedule> FilterBy(Guid jobId) =>
            _scheduler
                .GetTriggersOfJob(JobKey.Create(jobId.ToString()))
                .Select(Convert)
                .ToList();

        public IReadOnlyCollection<Schedule> FilterBy(string description) =>
            _scheduler
                .GetTriggerKeys(GroupMatcher<TriggerKey>.AnyGroup())
                .Select(k => _scheduler.GetTrigger(k))
                .Where(t => t.Description.StartsWith(description, true, CultureInfo.CurrentCulture))
                .Select(Convert)
                .ToList();

        private Schedule Convert(ITrigger trigger) => _converter.For(trigger);
    }
}