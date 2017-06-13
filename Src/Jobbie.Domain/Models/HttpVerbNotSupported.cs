using System;

namespace Jobbie.Domain.Models
{
    public sealed class HttpVerbNotSupported : Exception
    {
        public HttpVerbNotSupported(Job job)
            : base($"HTTP Verb '{job.HttpVerb}' for job ({job}) not supported.")
        {
            
        }
    }
}