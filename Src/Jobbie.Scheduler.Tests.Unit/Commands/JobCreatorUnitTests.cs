using System;
using Jobbie.Domain.Commands;
using Jobbie.Domain.Models;
using Jobbie.Infrastructure.Models;
using Jobbie.Scheduler.Commands;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoRhinoMock;
using Quartz;
using Rhino.Mocks;

namespace Jobbie.Scheduler.Tests.Unit.Commands
{
    [TestFixture]
    internal sealed class JobCreatorUnitTests
    {
        private TestContext _context;

        [SetUp]
        public void SetUp() => _context = new TestContext();

        [Test]
        public void Creates_job() =>
            _context
                .Act()
                .Assert();

        [Test]
        public void Fails_to_create_job() =>
            Assert.Throws<FailedToCreateJob>(() =>
                _context
                    .ArrangeFailedJobCreation()
                    .Act());

        private sealed class TestContext
        {
            private readonly IFixture _fixture;
            private readonly IJobCreator _sut;
            private readonly IScheduler _scheduler;
            private readonly INow _now;
            private readonly Guid _jobId;
            private readonly string _description;
            private readonly string _callbackUrl;
            private readonly string _httpVerb;
            private readonly string _payload;
            private readonly string _contentType;
            private readonly string _headers;

            public TestContext()
            {
                _fixture = new Fixture().Customize(new AutoRhinoMockCustomization());

                _jobId = _fixture.Create<Guid>();
                _description = _fixture.Create<string>();
                _callbackUrl = _fixture.Create<string>();
                _httpVerb = _fixture.Create<string>();
                _payload = _fixture.Create<string>();
                _contentType = _fixture.Create<string>();
                _headers = _fixture.Create<string>();

                _scheduler = _fixture.Freeze<IScheduler>();
                _now = _fixture.Freeze<INow>();
                _sut = _fixture.Create<JobCreator>();
            }

            public TestContext ArrangeFailedJobCreation()
            {
                _scheduler
                    .Expect(s => s.AddJob(null, true))
                    .IgnoreArguments()
                    .Throw(_fixture.Create<FailedToCreateJob>());
                return this;
            }

            public TestContext Act()
            {
                _sut.Create(_jobId, _description, _callbackUrl, _httpVerb, _payload, _contentType, _headers);
                return this;
            }

            public void Assert() =>
                _scheduler
                    .AssertWasCalled(s =>
                        s.AddJob(
                            Arg<IJobDetail>
                                .Matches(j =>
                                    Equals(j.Key, JobKey.Create(_jobId.ToString()))
                                    && j.Description == _description
                                    && j.Durable
                                    && j.JobDataMap.GetString("CallbackUrl") == _callbackUrl
                                    && j.JobDataMap.GetString("HttpVerb") == _httpVerb
                                    && j.JobDataMap.GetString("Payload") == _payload
                                    && j.JobDataMap.GetString("ContentType") == _contentType
                                    && j.JobDataMap.GetLong("CreatedUtc") == _now.Utc.Ticks
                                    && j.JobDataMap.GetString("Headers") == _headers
                                    && j.RequestsRecovery),
                            Arg<bool>.Is.Equal(true)));
        }
    }
}