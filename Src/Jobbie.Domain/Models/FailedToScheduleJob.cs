using System;

namespace Jobbie.Domain.Models
{
    public sealed class FailedToScheduleJob : Exception
    {
        public FailedToScheduleJob(Exception inner, Guid scheduleId, Guid jobId)
            : base($"Failed to schedule job (ScheduleId={scheduleId}|JobId={jobId}).", inner)
        {
            
        }
    }
}