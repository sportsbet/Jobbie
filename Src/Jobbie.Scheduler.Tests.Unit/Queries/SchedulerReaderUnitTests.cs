using System;
using FluentAssertions;
using Jobbie.Domain.Models;
using Jobbie.Domain.Queries;
using Jobbie.Scheduler.Queries;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoRhinoMock;
using Ploeh.SemanticComparison.Fluent;
using Quartz;
using Quartz.Impl.Triggers;
using Rhino.Mocks;

namespace Jobbie.Scheduler.Tests.Unit.Queries
{
    [TestFixture]
    internal sealed class SchedulerReaderUnitTests
    {
        private TestContext _context;

        [SetUp]
        public void SetUp() => _context = new TestContext();

        [Test]
        public void Finds_schedule() =>
            _context
                .ArrangeSchedule()
                .Act()
                .AssertScheduleWasFound();

        [Test]
        public void Fails_to_find_schedule() =>
            _context
                .Act()
                .AssertScheduleWasNotFound();

        private sealed class TestContext
        {
            private readonly IFixture _fixture;
            private readonly IScheduleReader _sut;
            private readonly IScheduler _scheduler;
            private readonly Schedule _expected;
            private readonly Guid _scheduleId;
            private Schedule _result;

            public TestContext()
            {
                _fixture = new Fixture().Customize(new AutoRhinoMockCustomization());

                _scheduleId = _fixture.Create<Guid>();
                _expected = _fixture.Create<Schedule.Builder>().Build();

                _fixture.Inject<ITrigger>(new SimpleTriggerImpl {Key = new TriggerKey(_scheduleId.ToString())});

                _scheduler = _fixture.Freeze<IScheduler>();
                var converter = _fixture.Freeze<IScheduleConverter>();
                _sut = _fixture.Create<ScheduleReader>();

                converter
                    .Expect(c => c.For(Arg<ITrigger>.Matches(t => Equals(t.Key, new TriggerKey(_scheduleId.ToString())))))
                    .Return(_expected);
            }

            public TestContext ArrangeSchedule()
            {
                _scheduler
                    .Expect(s => s.GetTrigger(new TriggerKey(_scheduleId.ToString())))
                    .Return(_fixture.Create<ITrigger>());
                return this;
            }

            public TestContext Act()
            {
                _result = _sut.For(_scheduleId);
                return this;
            }

            public void AssertScheduleWasFound() =>
                _expected
                    .AsSource()
                    .OfLikeness<Schedule>()
                    .ShouldEqual(_result);

            public void AssertScheduleWasNotFound() =>
                _result.Should().BeNull();
        }
    }
}