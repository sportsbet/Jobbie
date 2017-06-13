using System;
using Quartz;

namespace Jobbie.Executor.Models
{
    public sealed class JobFailedDuringExecution : Exception
    {
        public JobFailedDuringExecution(Exception inner, IJobDetail job, ITrigger trigger)
            : base($"Failed to execute job ({job.Key}|{job.Description}) on schedule ({trigger.Key}|{trigger.Description}).", inner)
        {
            
        }
    }
}