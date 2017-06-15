using System.Web.Http;
using Jobbie.Sample.Client.WebApi.Host.Contracts;
using System.Collections.Concurrent;
using System;

namespace Jobbie.Sample.Client.WebApi.Host.Controller
{
    public sealed class DestinationController : ApiController
    {
        private static readonly ConcurrentBag<JobExecution> _jobsExecuted = new ConcurrentBag<JobExecution>();

        public IHttpActionResult Get() => Ok(_jobsExecuted);

        public IHttpActionResult Create(JobExecution job)
        {
            Console.WriteLine($"Job ({job}) was received.");
            _jobsExecuted.Add(job);
            return Ok();
        }
    }
}