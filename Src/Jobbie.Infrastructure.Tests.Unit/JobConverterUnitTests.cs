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
                .Act()
                .Assert();

        private sealed class TestContext
        {
            private readonly IJobConverter _sut;
            private readonly IJobDetail _job;
            private readonly Job _expected;
            private Job _result;

            public TestContext()
            {
                var fixture = new Fixture().Customize(new AutoRhinoMockCustomization());

                var jobId = fixture.Create<Guid>();
                var description = fixture.Create<string>();
                var callbackUrl = fixture.Create<Uri>();
                var httpVerb = fixture.Create<HttpVerb>();
                var payload = fixture.Create<string>();
                var contentType = fixture.Create<string>();
                var createdUtc = fixture.Create<DateTime>();

                _job =
                    new JobDetailImpl(fixture.Create<string>(), typeof(JobExecutor))
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

                _expected = new Job(jobId, description, callbackUrl, httpVerb, payload, contentType, null, createdUtc);

                _sut = fixture.Create<JobConverter>();
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