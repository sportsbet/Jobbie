using System;
using System.Linq;
using FluentValidation;

namespace Jobbie.Sample.Scheduler.Contracts.Api.Payloads.Validation
{
    public sealed class JobCreateValidator : AbstractValidator<JobCreate>
    {
        private static readonly string[] _acceptableVerbs = {"post", "put", "delete"};

        public JobCreateValidator()
        {
            RuleFor(r => r.Description)
                .NotEmpty()
                .WithMessage("'Description' should not be empty.")
                .MaximumLength(250)
                .WithMessage("'Description' has a max. length of 250.");

            RuleFor(r => r.CallbackUrl)
                .NotEmpty()
                .WithMessage("'CallbackUrl' should not be empty.")
                .Must(
                    value =>
                    {
                        Uri temp;
                        return string.IsNullOrEmpty(value) || Uri.TryCreate(value, UriKind.Absolute, out temp);
                    })
                .WithMessage("'CallbackUrl' must contain a valid URI.");

            RuleFor(r => r.HttpVerb)
                .NotEmpty()
                .WithMessage("'HttpVerb' should not be empty.")
                .Must(value =>
                    string.IsNullOrEmpty(value)
                    || _acceptableVerbs.Contains(value, StringComparer.OrdinalIgnoreCase))
                .WithMessage($"'HttpVerb' must be one of {string.Join(", ", _acceptableVerbs)}.");

            RuleFor(r => r.Payload)
                .NotEmpty()
                .When(j =>
                    !string.IsNullOrEmpty(j.HttpVerb)
                    && !j.HttpVerb.Equals("delete", StringComparison.OrdinalIgnoreCase))
                .WithMessage("'Payload' should not be empty.");

            RuleFor(r => r.ContentType)
                .NotEmpty()
                .When(j =>
                    !string.IsNullOrEmpty(j.HttpVerb)
                    && !j.HttpVerb.Equals("delete", StringComparison.OrdinalIgnoreCase))
                .WithMessage("'ContentType' should not be empty.");
        }
    }
}