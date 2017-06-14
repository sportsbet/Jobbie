using System;
using FluentAssertions;
using Jobbie.Domain.Models;
using Jobbie.Domain.Queries;
using Jobbie.Executor.Commands;
using Jobbie.Infrastructure.Queries;
using Jobbie.Scheduler.Queries;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoRhinoMock;
using Ploeh.SemanticComparison.Fluent;
using Quartz;
using Quartz.Impl;
using Rhino.Mocks;

namespace Jobbie.Scheduler.Tests.Unit.Queries
{
    [TestFixture]
    internal sealed class JobReaderUnitTests
    {
        private TestContext _context;

        [SetUp]
        public void SetUp() => _context = new TestContext();

        [Test]
        public void Finds_job() =>
            _context
                .ArrangeJob()
                .Act()
                .AssertJobWasFound();

        [Test]
        public void Fails_to_find_job() =>
            _context
                .Act()
                .AssertJobWasNotFound();

        private sealed class TestContext
        {
            private readonly IFixture _fixture;
            private readonly IJobReader _sut;
            private readonly IScheduler _scheduler;
            private readonly Guid _jobId;
            private readonly Job _expected;
            private Job _result;

            public TestContext()
            {
                _fixture = new Fixture().Customize(new AutoRhinoMockCustomization());

                _jobId = _fixture.Create<Guid>();

                var jobDetail =
                    new JobDetailImpl(_fixture.Create<string>(), typeof (JobExecutor))
                    {
                        Key = JobKey.Create(_jobId.ToString())
                    };
                _fixture.Inject<IJobDetail>(jobDetail);

                _scheduler = _fixture.Freeze<IScheduler>();
                var converter = _fixture.Freeze<IJobConverter>();
                _sut = _fixture.Create<JobReader>();

                _expected =
                    new Job(
                        _jobId,
                        _fixture.Create<string>(),
                        _fixture.Create<Uri>(),
                        _fixture.Create<HttpVerb>(),
                        _fixture.Create<string>(),
                        _fixture.Create<string>(),
                        null,
                        _fixture.Create<DateTime>());
                converter
                    .Expect(c => c.For(Arg<IJobDetail>.Matches(j => Equals(j.Key, JobKey.Create(_jobId.ToString())))))
                    .Return(_expected);
            }

            public TestContext ArrangeJob()
            {
                _scheduler
                    .Expect(s => s.GetJobDetail(JobKey.Create(_jobId.ToString())))
                    .Return(_fixture.Create<IJobDetail>());
                return this;
            }

            public TestContext Act()
            {
                _result = _sut.For(_jobId);
                return this;
            }

            public void AssertJobWasFound() =>
                _expected
                    .AsSource()
                    .OfLikeness<Job>()
                    .ShouldEqual(_result);

            public void AssertJobWasNotFound() =>
                _result.Should().BeNull();
        }
    }
}