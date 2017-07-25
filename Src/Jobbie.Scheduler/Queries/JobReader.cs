using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Jobbie.Domain.Models;
using Jobbie.Domain.Queries;
using Jobbie.Infrastructure.Queries;
using Quartz;
using Quartz.Impl.Matchers;

namespace Jobbie.Scheduler.Queries
{
    public sealed class JobReader : IJobReader
    {
        private readonly IScheduler _scheduler;
        private readonly IJobConverter _converter;

        public JobReader(
            IScheduler scheduler,
            IJobConverter converter)
        {
            _scheduler = scheduler;
            _converter = converter;
        }

        public Job For(Guid jobId)
        {
            var job = _scheduler.GetJobDetail(new JobKey(jobId.ToString()));
            return job == null ? null : Convert(job);
        }

        public IReadOnlyCollection<Job> All() =>
            _scheduler
                .GetJobKeys(GroupMatcher<JobKey>.AnyGroup())
                .Select(k => _scheduler.GetJobDetail(k))
                .Select(Convert)
                .ToList();

        public IReadOnlyCollection<Job> FilterBy(string description) =>
            _scheduler
                .GetJobKeys(GroupMatcher<JobKey>.AnyGroup())
                .Select(k => _scheduler.GetJobDetail(k))
                .Where(j => j.Description.StartsWith(description, true, CultureInfo.CurrentCulture))
                .Select(Convert)
                .ToList();

        private Job Convert(IJobDetail job) => _converter.For(job);
    }
}