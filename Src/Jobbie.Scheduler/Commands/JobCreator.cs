using System;
using Common.Logging;
using Jobbie.Domain.Commands;
using Jobbie.Domain.Models;
using Jobbie.Executor.Commands;
using Jobbie.Infrastructure.Models;
using Quartz;

namespace Jobbie.Scheduler.Commands
{
    public sealed class JobCreator : IJobCreator
    {
        private static readonly ILog _log = LogManager.GetLogger<JobCreator>();

        private readonly IScheduler _scheduler;
        private readonly INow _now;

        public JobCreator(
            IScheduler scheduler,
            INow now)
        {
            _scheduler = scheduler;
            _now = now;
        }

        public void Create(
            Guid jobId,
            string description,
            string callbackUrl,
            string httpVerb,
            string payload,
            string contentType,
            string headers)
        {
            _log.Info($"[JobId={jobId}] [MessageText=Creating job (Description={description}).]");

            try
            {
                var details =
                    JobBuilder
                        .Create<JobExecutor>()
                        .WithIdentity(jobId.ToString())
                        .WithDescription(description)
                        .StoreDurably(true)
                        .UsingJobData("CallbackUrl", callbackUrl)
                        .UsingJobData("HttpVerb", httpVerb)
                        .UsingJobData("Payload", payload)
                        .UsingJobData("ContentType", contentType)
                        .UsingJobData("CreatedUtc", _now.Utc.Ticks)
                        .UsingJobData("Headers", headers)
                        .RequestRecovery(true)
                        .Build();

                _scheduler.AddJob(details, true);
            }
            catch (Exception e)
            {
                throw new FailedToCreateJob(e, jobId);
            }
        }
    }
}