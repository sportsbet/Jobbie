using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Jobbie.Domain.Commands;
using Jobbie.Domain.Queries;
using Jobbie.Sample.Scheduler.Contracts.Api;
using Jobbie.Sample.Scheduler.Contracts.Api.Payloads;
using Jobbie.Sample.Scheduler.Host.Infrastructure.WebApi;
using DomainSchedule = Jobbie.Domain.Models.Schedule;

namespace Jobbie.Sample.Scheduler.Host.Controllers
{
    public sealed class ScheduleController : ApiController
    {
        private readonly IScheduleReader _reader;
        private readonly IJobScheduler _scheduler;

        public ScheduleController(
            IScheduleReader reader,
            IJobScheduler scheduler)
        {
            _reader = reader;
            _scheduler = scheduler;
        }

        public IHttpActionResult Get([FromUri] Guid id)
        {
            var schedule = _reader.For(id);
            return schedule != null ? (IHttpActionResult) Ok(Convert(schedule)) : NotFound();
        }

        public IHttpActionResult Get([FromUri] Page page)
        {
            var schedules = OrderBy(_reader.All().Select(Convert), page).ToList();

            var pager = Pager(schedules, Relationships.Schedule_Query);
            return Ok(pager.GetPage(page));
        }

        public IHttpActionResult Get([FromUri] Guid jobId, [FromUri] Page page)
        {
            var schedules = OrderBy(_reader.FilterBy(jobId).Select(Convert), page).ToList();

            var pager = Pager(schedules, Relationships.Schedule_QueryBy_Job);
            return Ok(new ScheduleByJobPagedList(pager.GetPage(page), jobId));
        }

        public IHttpActionResult Get([FromUri] Page page, [FromUri] string description)
        {
            var schedules = OrderBy(_reader.FilterBy(description).Select(Convert), page).ToList();

            var pager = Pager(schedules, Relationships.Schedule_QueryBy_Description);
            return Ok(new ScheduleByDescriptionPagedList(pager.GetPage(page), description));
        }

        public IHttpActionResult Create([FromUri] Guid jobId, [FromBody] ScheduleCreate body)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var scheduleId = Guid.NewGuid();

            if (body.EndUtc.HasValue)
                _scheduler.Create(scheduleId, jobId, body.Description, body.StartUtc, body.Cron, body.EndUtc.Value);
            else if (!string.IsNullOrEmpty(body.Cron))
                _scheduler.Create(scheduleId, jobId, body.Description, body.StartUtc, body.Cron);
            else
                _scheduler.Create(scheduleId, jobId, body.Description, body.StartUtc);

            return Get(scheduleId);
        }

        public IHttpActionResult Delete([FromUri] Guid id)
        {
            if (_reader.For(id) == null)
                return NotFound();

            _scheduler.Delete(id);
            return Ok();
        }

        private static Schedule Convert(DomainSchedule s) =>
            new Schedule
            {
                ScheduleId = s.ScheduleId,
                JobId = s.JobId,
                Description = s.Description,
                CreatedUtc = s.CreatedUtc,
                StartUtc = s.StartUtc,
                NextUtc = s.NextUtc,
                PreviousUtc = s.PreviousUtc,
                Cron = s.Cron,
                EndUtc = s.EndUtc
            };

        private static IEnumerable<Schedule> OrderBy(IEnumerable<Schedule> schedules, IPage page) =>
            page.SortDirection == SortDirection.Ascending
                ? schedules.OrderBy(OrderBy(page))
                : schedules.OrderByDescending(OrderBy(page));

        private static Func<Schedule, object> OrderBy(IPage page)
        {
            switch (page.SortProperty?.ToLower() ?? string.Empty)
            {
                case "jobid":
                    return s => s.JobId;
                case "description":
                    return s => s.Description;
                case "startutc":
                    return s => s.StartUtc;
                case "nextutc":
                    return s => s.NextUtc;
                case "previousutc":
                    return s => s.PreviousUtc;
                case "cron":
                    return s => s.Cron;
                case "endutc":
                    return s => s.EndUtc;
                default:
                    return s => s.CreatedUtc;
            }
        }

        private static Pager<Schedule> Pager(IReadOnlyCollection<Schedule> schedules, string rel) =>
            new Pager<Schedule>(() => schedules, () => schedules.Count, rel, Curies.Jobbie);
    }
}
