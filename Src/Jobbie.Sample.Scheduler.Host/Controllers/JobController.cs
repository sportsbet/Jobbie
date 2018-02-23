using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Jobbie.Domain.Commands;
using Jobbie.Domain.Queries;
using Jobbie.Sample.Scheduler.Contracts.Api;
using Jobbie.Sample.Scheduler.Contracts.Api.Payloads;
using Jobbie.Sample.Scheduler.Host.Infrastructure.WebApi;
using DomainJob = Jobbie.Domain.Models.Job;

namespace Jobbie.Sample.Scheduler.Host.Controllers
{
    public sealed class JobController : ApiController
    {
        private readonly IJobReader _reader;
        private readonly IJobCreator _creator;
        private readonly IJobDeleter _deleter;

        public JobController(
            IJobReader reader,
            IJobCreator creator,
            IJobDeleter deleter)
        {
            _reader = reader;
            _creator = creator;
            _deleter = deleter;
        }

        public IHttpActionResult Get([FromUri] Guid id)
        {
            var job = _reader.For(id);
            return job != null ? (IHttpActionResult) Ok(Convert(job)) : NotFound();
        }

        public IHttpActionResult Get([FromUri] Page page)
        {
            var jobs = OrderBy(_reader.All().Select(Convert), page).ToList();

            var pager = Pager(jobs, Relationships.Job_Query);
            return Ok(pager.GetPage(page));
        }

        public IHttpActionResult Get([FromUri] string description, [FromUri] Page page)
        {
            var jobs = OrderBy(_reader.FilterBy(description).Select(Convert), page).ToList();

            var pager = Pager(jobs, Relationships.Job_QueryBy_Description);
            return Ok(new JobByDescriptionPagedList(pager.GetPage(page), description));
        }

        public IHttpActionResult Create([FromBody] JobCreate body)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var jobId = Guid.NewGuid();
            var timeout = Convert(body.TimeoutInMilliseconds);
            _creator.Create(jobId, body.Description, body.CallbackUrl, body.HttpVerb, body.Payload, body.ContentType, body.Headers, body.IsOnceOff, timeout);
            return Get(jobId);
        }

        public IHttpActionResult Delete([FromUri] Guid id)
        {
            if (_reader.For(id) == null)
                return NotFound();

            _deleter.Delete(id);
            return Ok();
        }

        private static Job Convert(DomainJob j) =>
            new Job
            {
                JobId = j.JobId,
                Description = j.Description,
                CallbackUrl = j.CallbackUrl.AbsoluteUri,
                HttpVerb = j.HttpVerb.ToString(),
                Payload = j.Payload,
                ContentType = j.ContentType,
                CreatedUtc = j.CreatedUtc,
                IsOnceOff = j.IsOnceOff,
                Timeout = j.Timeout
            };

        private static TimeSpan? Convert(int? timeoutInMilliseconds) =>
            timeoutInMilliseconds.HasValue
                ? TimeSpan.FromMilliseconds(timeoutInMilliseconds.Value)
                : (TimeSpan?) null;

        private static IEnumerable<Job> OrderBy(IEnumerable<Job> jobs, IPage page) =>
            page.SortDirection == SortDirection.Ascending
                ? jobs.OrderBy(OrderBy(page))
                : jobs.OrderByDescending(OrderBy(page));

        private static Func<Job, object> OrderBy(IPage page)
        {
            switch (page.SortProperty?.ToLower() ?? string.Empty)
            {
                case "description":
                    return s => s.Description;
                case "callbackurl":
                    return s => s.CallbackUrl;
                case "httppverb":
                    return s => s.HttpVerb;
                case "contenttype":
                    return s => s.ContentType;
                default:
                    return s => s.CreatedUtc;
            }
        }

        private static Pager<Job> Pager(IReadOnlyCollection<Job> jobs, string rel) =>
            new Pager<Job>(() => jobs, () => jobs.Count, rel, Curies.Jobbie);
    }
}
