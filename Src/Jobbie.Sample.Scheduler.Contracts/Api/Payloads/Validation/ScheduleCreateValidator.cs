using System;
using FluentValidation;

namespace Jobbie.Sample.Scheduler.Contracts.Api.Payloads.Validation
{
    public sealed class ScheduleCreateValidator : AbstractValidator<ScheduleCreate>
    {
        public ScheduleCreateValidator()
        {
            RuleFor(r => r.Description)
                .NotEmpty()
                .WithMessage("'Description' should not be empty.")
                .MaximumLength(250)
                .WithMessage("'Description' has a max. length of 250.");

            RuleFor(r => r.StartUtc)
                .NotEmpty()
                .WithMessage("'StartUtc' should not be empty.");

            RuleFor(r => r.Cron)
                .Matches(@"^\s*($|#|\w+\s*=|(\?|\*|(?:[0-5]?\d)(?:(?:-|\/|\,)(?:[0-5]?\d))?(?:,(?:[0-5]?\d)(?:(?:-|\/|\,)(?:[0-5]?\d))?)*)\s+(\?|\*|(?:[0-5]?\d)(?:(?:-|\/|\,)(?:[0-5]?\d))?(?:,(?:[0-5]?\d)(?:(?:-|\/|\,)(?:[0-5]?\d))?)*)\s+(\?|\*|(?:[01]?\d|2[0-3])(?:(?:-|\/|\,)(?:[01]?\d|2[0-3]))?(?:,(?:[01]?\d|2[0-3])(?:(?:-|\/|\,)(?:[01]?\d|2[0-3]))?)*)\s+(\?|\*|(?:0?[1-9]|[12]\d|3[01])(?:(?:-|\/|\,)(?:0?[1-9]|[12]\d|3[01]))?(?:,(?:0?[1-9]|[12]\d|3[01])(?:(?:-|\/|\,)(?:0?[1-9]|[12]\d|3[01]))?)*)\s+(\?|\*|(?:[1-9]|1[012])(?:(?:-|\/|\,)(?:[1-9]|1[012]))?(?:L|W)?(?:,(?:[1-9]|1[012])(?:(?:-|\/|\,)(?:[1-9]|1[012]))?(?:L|W)?)*|\?|\*|(?:JAN|FEB|MAR|APR|MAY|JUN|JUL|AUG|SEP|OCT|NOV|DEC)(?:(?:-)(?:JAN|FEB|MAR|APR|MAY|JUN|JUL|AUG|SEP|OCT|NOV|DEC))?(?:,(?:JAN|FEB|MAR|APR|MAY|JUN|JUL|AUG|SEP|OCT|NOV|DEC)(?:(?:-)(?:JAN|FEB|MAR|APR|MAY|JUN|JUL|AUG|SEP|OCT|NOV|DEC))?)*)\s+(\?|\*|(?:[0-6])(?:(?:-|\/|\,|#)(?:[0-6]))?(?:L)?(?:,(?:[0-6])(?:(?:-|\/|\,|#)(?:[0-6]))?(?:L)?)*|\?|\*|(?:MON|TUE|WED|THU|FRI|SAT|SUN)(?:(?:-)(?:MON|TUE|WED|THU|FRI|SAT|SUN))?(?:,(?:MON|TUE|WED|THU|FRI|SAT|SUN)(?:(?:-)(?:MON|TUE|WED|THU|FRI|SAT|SUN))?)*)(|\s)+(\?|\*|(?:|\d{4})(?:(?:-|\/|\,)(?:|\d{4}))?(?:,(?:|\d{4})(?:(?:-|\/|\,)(?:|\d{4}))?)*))$")
                .When(s => !string.IsNullOrEmpty(s.Cron))
                .WithMessage("'Cron' should be a valid cron expression.");

            RuleFor(r => r.EndUtc)
                .GreaterThan(DateTime.UtcNow)
                .When(j => j.EndUtc.HasValue)
                .WithMessage("'EndUtc' should not be in the past.");

            RuleFor(r => r.EndUtc)
                .GreaterThan(j => j.StartUtc)
                .When(j => j.EndUtc.HasValue)
                .WithMessage("'EndUtc' should not be before 'StartUtc'.");
        }
    }
}
