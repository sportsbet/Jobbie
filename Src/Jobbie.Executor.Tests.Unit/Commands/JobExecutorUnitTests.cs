using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Jobbie.Domain.Models;
using Jobbie.Executor.Commands;
using Jobbie.Executor.Models;
using Jobbie.Infrastructure.Http;
using Jobbie.Infrastructure.Queries;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoRhinoMock;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;
using Quartz.Spi;
using Rhino.Mocks;

namespace Jobbie.Executor.Tests.Unit.Commands
{
    [TestFixture]
    internal sealed class JobExecutorUnitTests
    {
        private TestContext _content;

        [SetUp]
        public void SetUp() => _content = new TestContext();

        [TestCase(false)]
        [TestCase(true)]
        public void Executes_job_with_http_post(bool timeout) =>
            _content
                .ArrangeTimeout(timeout)
                .ArrangePostJob()
                .Act()
                .Assert();

        [TestCase(false)]
        [TestCase(true)]
        public void Executes_job_with_http_put(bool timeout) =>
            _content
                .ArrangeTimeout(timeout)
                .ArrangePutJob()
                .Act()
                .Assert();

        [TestCase(false)]
        [TestCase(true)]
        public void Executes_job_with_http_delete(bool timeout) =>
            _content
                .ArrangeTimeout(timeout)
                .ArrangeDeleteJob()
                .Act()
                .Assert();

        [Test]
        public void Job_fails_execution_with_http_post() =>
            Assert.Throws<JobFailedDuringExecution>(() =>
                _content
                    .ArrangeFailedExecution()
                    .ArrangePostJob()
                    .Act());

        [Test]
        public void Job_fails_execution_with_http_put() =>
            Assert.Throws<JobFailedDuringExecution>(() =>
                _content
                    .ArrangeFailedExecution()
                    .ArrangePutJob()
                    .Act());

        [Test]
        public void Job_fails_execution_with_http_delete() =>
            Assert.Throws<JobFailedDuringExecution>(() =>
                _content
                    .ArrangeFailedExecution()
                    .ArrangeDeleteJob()
                    .Act());

        private sealed class TestContext
        {
            private readonly IFixture _fixture;
            private readonly JobExecutor _sut;
            private readonly IHttpClientWrapper _client;
            private readonly IJobConverter _converter;
            private readonly IJobExecutionContext _context;
            private Job _job;
            private TimeSpan? _timeout;

            public TestContext()
            {
                _fixture = new Fixture().Customize(new AutoRhinoMockCustomization());

                _fixture.Inject<IJobDetail>(new JobDetailImpl(_fixture.Create<string>(), typeof (JobExecutor)));
                _fixture.Inject<IOperableTrigger>(new SimpleTriggerImpl(_fixture.Create<string>()));
                var trigger = _fixture.Create<TriggerFiredBundle>();
                var scheduler = _fixture.Create<IScheduler>();
                var job = _fixture.Create<IJob>();
                _context = new JobExecutionContextImpl(scheduler, trigger, job);

                _fixture.Customize<HttpResponseMessage>(c => c.With(m => m.StatusCode, HttpStatusCode.OK));

                _client = _fixture.Freeze<IHttpClientWrapper>();
                _converter = _fixture.Freeze<IJobConverter>();
                _sut = _fixture.Create<JobExecutor>();
            }

            public TestContext ArrangePostJob()
            {
                ArrangeJob(HttpVerb.Post);

                _client
                    .Expect(c => c.PostAsync(Arg<Uri>.Is.Equal(_job.CallbackUrl), Arg<StringContent>.Is.Anything))
                    .Return(Task.FromResult(_fixture.Create<HttpResponseMessage>()));

                if (_timeout.HasValue)
                    _client.Expect(c => c.Timeout).SetPropertyWithArgument(_timeout.Value);

                return this;
            }

            public TestContext ArrangePutJob()
            {
                ArrangeJob(HttpVerb.Put);

                _client
                    .Expect(c => c.PutAsync(Arg<Uri>.Is.Equal(_job.CallbackUrl), Arg<StringContent>.Is.Anything))
                    .Return(Task.FromResult(_fixture.Create<HttpResponseMessage>()));

                return this;
            }

            public TestContext ArrangeDeleteJob()
            {
                ArrangeJob(HttpVerb.Delete);

                _client
                    .Expect(c => c.DeleteAsync(Arg<Uri>.Is.Equal(_job.CallbackUrl)))
                    .Return(Task.FromResult(_fixture.Create<HttpResponseMessage>()));

                return this;
            }

            public TestContext ArrangeFailedExecution()
            {
                _fixture.Customize<HttpResponseMessage>(c => c.With(m => m.StatusCode, HttpStatusCode.InternalServerError));
                return this;
            }

            public TestContext ArrangeTimeout(bool timeout)
            {
                if (timeout)
                    _timeout = _fixture.Create<TimeSpan>();
                return this;
            }

            public TestContext Act()
            {
                _sut.Execute(_context);
                return this;
            }

            public void Assert() => _client.VerifyAllExpectations();

            private void ArrangeJob(HttpVerb httpVerb)
            {
                var jobId = _context.JobDetail.Key.Name;
                var jobKey = JobKey.Create(jobId);

                _job =
                    new Job(
                        new Guid(jobId),
                        _fixture.Create<string>(),
                        _fixture.Create<Uri>(),
                        httpVerb,
                        _fixture.Create<string>(),
                        "application/json",
                        null,
                        _fixture.Create<DateTime>(),
                        _timeout);

                _converter
                    .Expect(c => c.For(Arg<IJobDetail>.Matches(j => Equals(_context.JobDetail.Key, jobKey))))
                    .Return(_job);
            }
        }
    }
}
