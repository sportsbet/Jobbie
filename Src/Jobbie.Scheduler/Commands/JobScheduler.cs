using System;
using Common.Logging;
using Jobbie.Domain.Commands;
using Jobbie.Domain.Models;
using Jobbie.Infrastructure.Models;
using Quartz;

namespace Jobbie.Scheduler.Commands
{
    public sealed class JobScheduler : IJobScheduler
    {
        private static readonly ILog _log = LogManager.GetLogger<JobScheduler>();

        private readonly IScheduler _scheduler;
        private readonly INow _now;

        public JobScheduler(
            IScheduler scheduler,
            INow now)
        {
            _scheduler = scheduler;
            _now = now;
        }

        public void Create(Guid scheduleId, Guid jobId, string description, DateTime startUtc)
        {
            try
            {
                var trigger =
                    Construct(scheduleId, jobId, description, startUtc)
                        .Build();

                _scheduler.ScheduleJob(trigger);
            }
            catch (Exception e)
            {
                throw new FailedToScheduleJob(e, scheduleId, jobId);
            }
        }

        public void Create(Guid scheduleId, Guid jobId, string description, DateTime startUtc, string cron)
        {
            try
            {
                var trigger =
                    Construct(scheduleId, jobId, description, startUtc)
                        .WithCronSchedule(cron)
                        .UsingJobData("Cron", cron)
                        .Build();

                _scheduler.ScheduleJob(trigger);
            }
            catch (Exception e)
            {
                throw new FailedToScheduleJob(e, scheduleId, jobId);
            }
        }

        public void Create(Guid scheduleId, Guid jobId, string description, DateTime startUtc, string cron, DateTime endUtc)
        {
            try
            {
                var trigger =
                    Construct(scheduleId, jobId, description, startUtc)
                        .WithCronSchedule(cron)
                        .UsingJobData("Cron", cron)
                        .EndAt(endUtc)
                        .Build();

                _scheduler.ScheduleJob(trigger);
            }
            catch (Exception e)
            {
                throw new FailedToScheduleJob(e, scheduleId, jobId);
            }
        }

        public void Delete(Guid scheduleId)
        {
            _log.Info($"[ScheduleId={scheduleId}] [MessageText=Deleting schedule.]");

            try
            {
                _scheduler.UnscheduleJob(new TriggerKey(scheduleId.ToString()));
            }
            catch (Exception e)
            {
                throw new FailedToDeleteSchedule(e, scheduleId);
            }
        }

        private TriggerBuilder Construct(Guid scheduleId, Guid jobId, string description, DateTime startUtc)
        {
            _log.Info($"[JobId={jobId}] [ScheduleId={scheduleId}] [MessageText=Creating schedule (Description={description}|StartUtc={startUtc}).]");

            return
                TriggerBuilder
                    .Create()
                    .WithIdentity(scheduleId.ToString())
                    .ForJob(jobId.ToString())
                    .WithDescription(description)
                    .StartAt(startUtc)
                    .UsingJobData("CreatedUtc", _now.Utc.Ticks);
        }
    }
}