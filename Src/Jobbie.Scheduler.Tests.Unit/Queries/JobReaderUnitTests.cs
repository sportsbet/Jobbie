using System;
using System.Linq;
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
using Quartz.Collection;
using Quartz.Impl;
using Quartz.Impl.Matchers;
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
                .ActFor()
                .AssertJobWasFound();

        [Test]
        public void Fails_to_find_job() =>
            _context
                .ActFor()
                .AssertJobWasNotFound();

        [Test]
        public void Does_not_return_invalid_job() =>
            _context
                .ArrangeInvalidJob()
                .ActFor()
                .AssertJobWasNotFound();

        [Test]
        public void Finds_all_jobs() =>
            _context
                .ArrangeJobs()
                .ActAll()
                .AssertJobWasFound();

        [Test]
        public void Finds_jobs_by_description() =>
            _context
                .ArrangeJobs()
                .ActFilterBy()
                .AssertJobWasFound();

        private sealed class TestContext
        {
            private readonly IFixture _fixture;
            private readonly IJobReader _sut;
            private readonly IScheduler _scheduler;
            private readonly IJobConverter _converter;
            private readonly Guid _jobId;
            private readonly Job _expected;
            private readonly string _description;
            private Job _result;

            public TestContext()
            {
                _fixture = new Fixture().Customize(new AutoRhinoMockCustomization());

                _jobId = _fixture.Create<Guid>();
                _description = _fixture.Create<string>();

                var jobDetail =
                    new JobDetailImpl(_fixture.Create<string>(), typeof (JobExecutor))
                    {
                        Key = JobKey.Create(_jobId.ToString()),
                        Description = _description
                    };
                _fixture.Inject<IJobDetail>(jobDetail);

                _scheduler = _fixture.Freeze<IScheduler>();
                _converter = _fixture.Freeze<IJobConverter>();
                _sut = _fixture.Create<JobReader>();

                _expected =
                    new Job(
                        _jobId,
                        _description,
                        _fixture.Create<Uri>(),
                        _fixture.Create<HttpVerb>(),
                        _fixture.Create<string>(),
                        _fixture.Create<string>(),
                        null,
                        _fixture.Create<DateTime>(),
                        _fixture.Create<bool>());
            }

            public TestContext ArrangeJob()
            {
                _scheduler
                    .Expect(s => s.GetJobDetail(JobKey.Create(_jobId.ToString())))
                    .Return(_fixture.Create<IJobDetail>());

                _converter
                    .Expect(c => c.For(Arg<IJobDetail>.Matches(j => Equals(j.Key, JobKey.Create(_jobId.ToString())))))
                    .Return(_expected);
                return this;
            }

            public TestContext ArrangeInvalidJob()
            {
                _scheduler
                    .Expect(s => s.GetJobDetail(JobKey.Create(_jobId.ToString())))
                    .Return(_fixture.Create<IJobDetail>());

                _converter
                    .Expect(c => c.For(Arg<IJobDetail>.Matches(j => Equals(j.Key, JobKey.Create(_jobId.ToString())))))
                    .Throw(_fixture.Create<Exception>());
                return this;
            }

            public TestContext ArrangeJobs()
            {
                var key = JobKey.Create(_jobId.ToString());

                _scheduler
                    .Expect(s => s.GetJobKeys(GroupMatcher<JobKey>.AnyGroup()))
                    .Return(new HashSet<JobKey>(new[] {key}));

                _scheduler
                    .Expect(s => s.GetJobDetail(Arg<JobKey>.Is.Anything))
                    .Return(_fixture.Create<IJobDetail>());

                _converter
                    .Expect(c => c.For(Arg<IJobDetail>.Matches(j => Equals(j.Key, key))))
                    .Return(_expected);
                return this;
            }

            public TestContext ActFor()
            {
                _result = _sut.For(_jobId);
                return this;
            }

            public TestContext ActAll()
            {
                _result = _sut.All().FirstOrDefault();
                return this;
            }

            public TestContext ActFilterBy()
            {
                _result = _sut.FilterBy(_description).FirstOrDefault();
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