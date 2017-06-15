using FluentValidation.Attributes;
using Jobbie.Sample.Scheduler.Contracts.Api.Payloads.Validation;

namespace Jobbie.Sample.Scheduler.Contracts.Api.Payloads
{
    [Validator(typeof(JobCreateValidator))]
    public sealed class JobCreate
    {
        public string Description { get; set; }

        public string CallbackUrl { get; set; }

        public string HttpVerb { get; set; }

        public string Payload { get; set; }

        public string ContentType { get; set; }

        public string Headers { get; set; }
    }
}