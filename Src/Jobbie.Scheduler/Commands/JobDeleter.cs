using System;
using Common.Logging;
using Jobbie.Domain.Commands;
using Jobbie.Domain.Models;
using Quartz;

namespace Jobbie.Scheduler.Commands
{
    public sealed class JobDeleter : IJobDeleter
    {
        private static readonly ILog _log = LogManager.GetLogger<JobDeleter>();

        private readonly IScheduler _scheduler;

        public JobDeleter(
            IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public void Delete(Guid jobId)
        {
            _log.Info($"[JobId={jobId}] [MessageText=Deleting job.]");

            try
            {
                _scheduler.DeleteJob(JobKey.Create(jobId.ToString()));
            }
            catch (Exception e)
            {
                throw new FailedToDeleteJob(e, jobId);
            }
        }
    }
}