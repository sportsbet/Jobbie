using System;

namespace Jobbie.Domain.Models
{
    public sealed class FailedToCreateJob : Exception
    {
        public FailedToCreateJob(Exception inner, Guid jobId)
            : base($"Failed to create job ({jobId}).", inner)
        {
            
        }
    }
}