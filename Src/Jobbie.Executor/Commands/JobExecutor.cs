using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using Common.Logging;
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

        public JobExecutor(
            IHttpClientWrapper client,
            IJobConverter converter,
            Stopwatch timer)
        {
            _client = client;
            _converter = converter;
            _timer = timer;
        }

        public void Execute(IJobExecutionContext context)
        {
            var raw = context.JobDetail;
            var trigger = context.Trigger;
            _log.Info($"Executing job ({raw.Key}|{raw.Description}) on schedule ({trigger.Key}|{trigger.Description}).");
            _timer.Start();
            
            try
            {
                var job = _converter.For(raw);

                AddHeaders(job);

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
            }
            catch (Exception e)
            {
                var failure = new JobFailedDuringExecution(e, raw, trigger);
                _log.Error(failure);
                throw failure;
            }

            _log.Info($"Finished executing job ({raw.Key}|{raw.Description}) on schedule ({trigger.Key}|{trigger.Description}). TimeTaken={_timer.Elapsed}.");
        }

        private void Post(Job job)
        {
            _log.Debug($"Sending HTTP POST request to {job.CallbackUrl}.");
            var content = new StringContent(job.Payload, Encoding.UTF8, job.ContentType);
            var request = _client.PostAsync(job.CallbackUrl, content).Result;
            request.EnsureSuccessStatusCode();
        }

        private void Put(Job job)
        {
            _log.Debug($"Sending HTTP PUT request to {job.CallbackUrl}.");
            var content = new StringContent(job.Payload, Encoding.UTF8, job.ContentType);
            var request = _client.PutAsync(job.CallbackUrl, content).Result;
            request.EnsureSuccessStatusCode();
        }

        private void Delete(Job job)
        {
            _log.Debug($"Sending HTTP DELETE request to {job.CallbackUrl}.");
            var request = _client.DeleteAsync(job.CallbackUrl).Result;
            request.EnsureSuccessStatusCode();
        }

        private void AddHeaders(Job job)
        {
            foreach (var header in job.Headers)
                _client.AddHeader(header.Key, header.Value);
        }
    }
}