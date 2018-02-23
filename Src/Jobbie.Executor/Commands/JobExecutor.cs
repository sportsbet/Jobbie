using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using Common.Logging;
using Jobbie.Domain.Commands;
using Jobbie.Domain.Models;
using Jobbie.Executor.Models;
using Jobbie.Infrastructure.Http;
using Jobbie.Infrastructure.Queries;
using Quartz;

namespace Jobbie.Executor.Commands
{
    public sealed class JobExecutor : IJob
    {
        private static readonly ILog _log = LogManager.GetLogger<JobExecutor>();

        private readonly IHttpClientWrapper _client;
        private readonly IJobConverter _converter;
        private readonly Stopwatch _timer;
        private readonly IJobDeleter _cleanup;

        public JobExecutor(
            IHttpClientWrapper client,
            IJobConverter converter,
            Stopwatch timer,
            IJobDeleter cleanup)
        {
            _client = client;
            _converter = converter;
            _timer = timer;
            _cleanup = cleanup;
        }

        public void Execute(IJobExecutionContext context)
        {
            var raw = context.JobDetail;
            var trigger = context.Trigger;
            _log.Info($"[JobId={raw.Key}] [ScheduleId={trigger.Key}] [MessageText=Started executing job ({raw.Description}) on schedule ({trigger.Description}).]");
            _timer.Start();
            
            try
            {
                var job = _converter.For(raw);

                AddHeaders(job);

                SetTimeout(job);

                switch (job.HttpVerb)
                {
                    case HttpVerb.Post:
                        Post(job);
                        break;
                    case HttpVerb.Put:
                        Put(job);
                        break;
                    case HttpVerb.Delete:
                        Delete(job);
                        break;
                    default:
                        throw new HttpVerbNotSupported(job);
                }

                if (job.IsOnceOff)
                {
                    _log.Info($"[JobId={raw.Key}] [ScheduleId={trigger.Key}] [TimeTaken={_timer.Elapsed}] [MessageText=Deleting once-off job ({raw.Description}) on schedule ({trigger.Description}).]");
                    _cleanup.Delete(job.JobId);
                }
            }
            catch (Exception e)
            {
                var failure = new JobFailedDuringExecution(e, raw, trigger);
                _log.Error(failure);
                throw failure;
            }

            _log.Info($"[JobId={raw.Key}] [ScheduleId={trigger.Key}] [TimeTaken={_timer.Elapsed}] [MessageText=Finished executing job ({raw.Description}) on schedule ({trigger.Description}).]");
        }

        private void Post(Job job)
        {
            _log.Debug($"[JobId={job.JobId}] [MessageText=Sending HTTP POST request to {job.CallbackUrl}.]");
            var content = new StringContent(job.Payload, Encoding.UTF8, job.ContentType);
            var request = _client.PostAsync(job.CallbackUrl, content).Result;
            request.EnsureSuccessStatusCode();
        }

        private void Put(Job job)
        {
            _log.Debug($"[JobId={job.JobId}] [MessageText=Sending HTTP PUT request to {job.CallbackUrl}.]");
            var content = new StringContent(job.Payload, Encoding.UTF8, job.ContentType);
            var request = _client.PutAsync(job.CallbackUrl, content).Result;
            request.EnsureSuccessStatusCode();
        }

        private void Delete(Job job)
        {
            _log.Debug($"[JobId={job.JobId}] [MessageText=Sending HTTP DELETE request to {job.CallbackUrl}.]");
            var request = _client.DeleteAsync(job.CallbackUrl).Result;
            request.EnsureSuccessStatusCode();
        }

        private void AddHeaders(Job job)
        {
            foreach (var header in job.Headers)
                _client.AddHeader(header.Key, header.Value);
        }

        private void SetTimeout(Job job)
        {
            if (job.Timeout.HasValue)
                _client.Timeout = job.Timeout.Value;
        }
    }
}