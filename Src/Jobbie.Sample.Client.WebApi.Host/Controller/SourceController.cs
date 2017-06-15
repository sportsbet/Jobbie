using System;
using System.Web.Http;
using HoneyBear.HalClient;
using Jobbie.Sample.Client.WebApi.Host.Contracts;
using Jobbie.Sample.Scheduler.Contracts.Api;
using Jobbie.Sample.Scheduler.Contracts.Api.Payloads;

namespace Jobbie.Sample.Client.WebApi.Host.Controller
{
    public sealed class SourceController : ApiController
    {
        private readonly IHalClient _client;

        public SourceController(
            IHalClient client)
        {
            _client = client;
        }

        public IHttpActionResult Create(JobExecution body)
        {
            var jobBody =
                new JobCreate
                {
                    Description = body.Description,
                    CallbackUrl = "http://localhost:31901/destination",
                    HttpVerb = "POST",
                    Payload = $"{{ \"description\": \"{body.Description}\" }}",
                    ContentType = "application/json",
                    Headers = "source=Jobbie.Sample.Client.WebApi.Host"
                };

            var scheduleBody =
                new ScheduleCreate
                {
                    Description = body.Description,
                    StartUtc = DateTime.UtcNow.AddSeconds(10),
                    Cron = body.Cron
                };

            var schedule =
                _client
                    .Root()
                    .Post(Relationships.Job_Create, jobBody, Curies.Jobbie)
                    .Post(Relationships.Schedule_Create, scheduleBody, Curies.Jobbie)
                    .Item<Schedule>()
                    .Data;

            return Ok(schedule);
        }
    }
}
