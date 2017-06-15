using System;
using FluentValidation.Attributes;
using Jobbie.Sample.Scheduler.Contracts.Api.Payloads.Validation;

namespace Jobbie.Sample.Scheduler.Contracts.Api.Payloads
{
    [Validator(typeof(ScheduleCreateValidator))]
    public sealed class ScheduleCreate
    {
        public string Description { get; set; }

        public DateTime StartUtc { get; set; }

        public string Cron { get; set; }

        public DateTime? EndUtc { get; set; }
    }
}