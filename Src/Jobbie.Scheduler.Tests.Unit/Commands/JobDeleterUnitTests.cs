using System;
using Jobbie.Domain.Commands;
using Jobbie.Domain.Models;
using Jobbie.Scheduler.Commands;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoRhinoMock;
using Quartz;
using Rhino.Mocks;

namespace Jobbie.Scheduler.Tests.Unit.Commands
{
    [TestFixture]
    internal sealed class JobDeleterUnitTests
    {
        private TestContext _context;

        [SetUp]
        public void SetUp() => _context = new TestContext();

        [Test]
        public void Deletes_job() =>
            _context
                .Act()
                .Assert();

        [Test]
        public void Fails_to_delete_job() =>
            Assert.Throws<FailedToDeleteJob>(() =>
                _context
                    .ArrangeFailedJobDeletion()
                    .Act());

        private sealed class TestContext
        {
            private readonly IFixture _fixture;
            private readonly IScheduler _scheduler;
            private readonly IJobDeleter _sut;
            private readonly Guid _jobId;

            public TestContext()
            {
                _fixture = new Fixture().Customize(new AutoRhinoMockCustomization());

                _jobId = _fixture.Create<Guid>();
                _scheduler = _fixture.Freeze<IScheduler>();
                _sut = _fixture.Create<JobDeleter>();
            }

            public TestContext ArrangeFailedJobDeletion()
            {
                _scheduler
                    .Expect(s => s.DeleteJob(JobKey.Create(_jobId.ToString())))
                    .Throw(_fixture.Create<Exception>());
                return this;
            }

            public TestContext Act()
            {
                _sut.Delete(_jobId);
                return this;
            }

            public void Assert() =>
                _scheduler
                    .AssertWasCalled(s => s.DeleteJob(JobKey.Create(_jobId.ToString())));
        }
    }
}