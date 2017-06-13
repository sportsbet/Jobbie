using System;

namespace Jobbie.Domain.Models
{
    public sealed class FailedToDeleteJob : Exception
    {
        public FailedToDeleteJob(Exception inner, Guid jobId)
            : base($"Failed to delete job ({jobId}).", inner)
        {

        }
    }
}