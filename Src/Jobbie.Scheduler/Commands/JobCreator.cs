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
            string headers,
            bool isOnceOff) =>
            Create(jobId, description, callbackUrl, httpVerb, payload, contentType, headers, isOnceOff, null);

        public void Create(
            Guid jobId,
            string description,
            string callbackUrl,
            string httpVerb,
            string payload,
            string contentType,
            string headers,
            bool isOnceOff,
            TimeSpan? timeout)
        {
            _log.Info($"[JobId={jobId}] [MessageText=Creating job (Description={description}).]");

            try
            {
                var builder =
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
                        .UsingJobData("IsOnceOff", isOnceOff)
                        .RequestRecovery(false);

                if (timeout.HasValue)
                    builder.UsingJobData("Timeout", timeout.Value.Ticks);

                _scheduler.AddJob(builder.Build(), true);
            }
            catch (Exception e)
            {
                throw new FailedToCreateJob(e, jobId);
            }
        }
    }
}