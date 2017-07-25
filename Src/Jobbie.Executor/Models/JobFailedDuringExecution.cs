using System;
using Quartz;

namespace Jobbie.Executor.Models
{
    public sealed class JobFailedDuringExecution : Exception
    {
        public JobFailedDuringExecution(Exception inner, IJobDetail job, ITrigger trigger)
            : base($"[JobId={job.Key}] [ScheduleId={trigger.Key}] [MessageText=Failed to execute job ({job.Description}) on schedule ({trigger.Description}).]", inner)
        {
            
        }
    }
}