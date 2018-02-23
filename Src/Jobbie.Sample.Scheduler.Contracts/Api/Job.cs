using System;
using WebApi.Hal;

namespace Jobbie.Sample.Scheduler.Contracts.Api
{
    public sealed class Job : Representation
    {
        public Guid JobId { get; set; }

        public string Description { get; set; }

        public string CallbackUrl { get; set; }

        public string HttpVerb { get; set; }

        public string Payload { get; set; }

        public string ContentType { get; set; }

        public DateTime CreatedUtc { get; set; }

        public bool IsOnceOff { get; set; }

        public TimeSpan? Timeout { get; set; }

        public override string ToString() => $"JobId={JobId}";
    }
}