using System;
using Jobbie.Domain.Models;
using Jobbie.Executor.Commands;
using Jobbie.Infrastructure.Queries;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoRhinoMock;
using Ploeh.SemanticComparison.Fluent;
using Quartz;
using Quartz.Impl;

namespace Jobbie.Infrastructure.Tests.Unit
{
    [TestFixture]
    internal sealed class JobConverterUnitTests
    {
        private TestContext _context;

        [SetUp]
        public void SetUp() => _context = new TestContext();

        [Test]
        public void Converts_Job() =>
            _context
                .ArrangeJob()
                .Act()
                .Assert();

        [Test]
        public void Converts_Job_with_timeout() =>
            _context
                .ArrangeTimeout()
                .ArrangeJob()
                .Act()
                .Assert();

        private sealed class TestContext
        {
            private readonly IFixture _fixture;
            private readonly IJobConverter _sut;
            private IJobDetail _job;
            private Job _expected;
            private Job _result;
            private TimeSpan? _timeout;

            public TestContext()
            {
                _fixture = new Fixture().Customize(new AutoRhinoMockCustomization());

                _sut = _fixture.Create<JobConverter>();
            }

            public TestContext ArrangeTimeout()
            {
                _timeout = _fixture.Create<TimeSpan>();
                return this;
            }

            public TestContext ArrangeJob()
            {
                var jobId = _fixture.Create<Guid>();
                var description = _fixture.Create<string>();
                var callbackUrl = _fixture.Create<Uri>();
                var httpVerb = _fixture.Create<HttpVerb>();
                var payload = _fixture.Create<string>();
                var contentType = _fixture.Create<string>();
                var createdUtc = _fixture.Create<DateTime>();
                var isOnceOff = _fixture.Create<bool>();

                _job =
                    new JobDetailImpl(_fixture.Create<string>(), typeof(JobExecutor))
                    {
                        Key = JobKey.Create(jobId.ToString()),
                        Description = description,
                        JobDataMap =
                            new JobDataMap
                            {
                                {"CallbackUrl", callbackUrl.AbsoluteUri},
                                {"HttpVerb", httpVerb.ToString()},
                                {"Payload", payload},
                                {"ContentType", contentType},
                                {"CreatedUtc", createdUtc.Ticks}
                            }
                    };

                if (_timeout.HasValue)
                    _job.JobDataMap.Add("Timeout", _timeout.Value.Ticks);

                _expected = new Job(jobId, description, callbackUrl, httpVerb, payload, contentType, null, createdUtc, isOnceOff, _timeout);
                return this;
            }

            public TestContext Act()
            {
                _result = _sut.For(_job);
                return this;
            }

            public void Assert() =>
                _expected
                    .AsSource()
                    .OfLikeness<Job>()
                    .Without(j => j.Headers)
                    .ShouldEqual(_result);
        }
    }
}