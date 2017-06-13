using System;

namespace Jobbie.Domain.Models
{
    public sealed class FailedToDeleteSchedule : Exception
    {
        public FailedToDeleteSchedule(Exception inner, Guid scheduleId)
            : base($"Failed to delete schedule ({scheduleId}).", inner)
        {
            
        }
    }
}