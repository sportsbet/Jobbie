using System;
using Jobbie.Domain.Models;
using Quartz;

namespace Jobbie.Infrastructure.Queries
{
    public sealed class JobConverter : IJobConverter
    {
        public Job For(IJobDetail job) =>
            new Job(
                new Guid(job.Key.Name),
                job.Description,
                new Uri(job.JobDataMap.GetString("CallbackUrl")),
                HttpVerb(job.JobDataMap.GetString("HttpVerb")),
                job.JobDataMap.GetString("Payload"),
                job.JobDataMap.GetString("ContentType"),
                job.JobDataMap.GetString("Headers"),
                new DateTime(job.JobDataMap.GetLong("CreatedUtc")),
                Timeout(job.JobDataMap));

        private static HttpVerb HttpVerb(string value)
        {
            Enum.TryParse<HttpVerb>(value, true, out var verb);
            return verb;
        }

        private static TimeSpan? Timeout(JobDataMap job) =>
            job.ContainsKey("Timeout")
                ? TimeSpan.FromTicks(job.GetIntValue("Timeout"))
                : (TimeSpan?) null;
    }
}