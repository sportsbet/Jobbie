using System;
using Jobbie.Domain.Models;
using Jobbie.Scheduler.Queries;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoRhinoMock;
using Ploeh.SemanticComparison.Fluent;
using Quartz;
using Quartz.Impl.Triggers;

namespace Jobbie.Scheduler.Tests.Unit.Queries
{
    [TestFixture]
    internal sealed class ScheduleConverterUnitTests
    {
        private TestContext _context;

        [SetUp]
        public void SetUp() => _context = new TestContext();

        [Test]
        public void Converts_once_off_schedule() =>
            _context
                .ArrangeOnceOffSchedule()
                .Act()
                .Assert();

        [Test]
        public void Converts_never_ending_recurring_schedule() =>
            _context
                .ArrangeNeverEndingRecurringSchedule()
                .Act()
                .Assert();

        [Test]
        public void Converts_finite_recurring_schedule() =>
            _context
                .ArrangeFiniteRecurringSchedule()
                .Act()
                .Assert();

        private sealed class TestContext
        {
            private readonly IFixture _fixture;
            private readonly IScheduleConverter _sut;
            private ITrigger _trigger;
            private readonly Schedule.Builder _expected;
            private Schedule _result;
            private readonly Guid _scheduleId;
            private readonly Guid _jobId;
            private readonly string _description;
            private readonly DateTime _createdUtc;
            private readonly DateTime _startUtc;

            public TestContext()
            {
                _fixture = new Fixture().Customize(new AutoRhinoMockCustomization());

                _scheduleId = _fixture.Create<Guid>();
                _jobId = _fixture.Create<Guid>();
                _description = _fixture.Create<string>();
                _createdUtc = _fixture.Create<DateTime>();
                _startUtc = _fixture.Create<DateTimeOffset>().UtcDateTime;

                _expected = Schedule.Init(_scheduleId, _jobId, _description, _createdUtc, _startUtc);

                _sut = _fixture.Create<ScheduleConverter>();
            }

            public TestContext ArrangeOnceOffSchedule()
            {
                _trigger =
                    new SimpleTriggerImpl
                    {
                        Key = new TriggerKey(_scheduleId.ToString()),
                        JobKey = JobKey.Create(_jobId.ToString()),
                        Description = _description,
                        StartTimeUtc = _startUtc,
                        JobDataMap = new JobDataMap {{"CreatedUtc", _createdUtc.Ticks}}
                    };
                return this;
            }

            public TestContext ArrangeNeverEndingRecurringSchedule()
            {
                const string cron = "* * * ? * *";
                _trigger =
                    new CronTriggerImpl
                    {
                        Key = new TriggerKey(_scheduleId.ToString()),
                        JobKey = JobKey.Create(_jobId.ToString()),
                        Description = _description,
                        StartTimeUtc = _startUtc,
                        JobDataMap =
                            new JobDataMap
                            {
                                {"CreatedUtc", _createdUtc.Ticks},
                                {"Cron", cron}
                            },
                        CronExpressionString = cron
                    };
                _expected
                    .WithCron(cron);
                return this;
            }

            public TestContext ArrangeFiniteRecurringSchedule()
            {
                const string cron = "* * * ? * *";
                var endUtc = _startUtc.Add(_fixture.Create<TimeSpan>());
                _trigger =
                    new CronTriggerImpl
                    {
                        Key = new TriggerKey(_scheduleId.ToString()),
                        JobKey = JobKey.Create(_jobId.ToString()),
                        Description = _description,
                        StartTimeUtc = _startUtc,
                        JobDataMap =
                            new JobDataMap
                            {
                                {"CreatedUtc", _createdUtc.Ticks},
                                {"Cron", cron}
                            },
                        CronExpressionString = cron,
                        EndTimeUtc = endUtc
                    };
                _expected
                    .WithCron(cron)
                    .WithEndUtc(endUtc);
                return this;
            }

            public TestContext Act()
            {
                _result = _sut.For(_trigger);
                return this;
            }

            public void Assert() =>
                _expected
                    .Build()
                    .AsSource()
                    .OfLikeness<Schedule>()
                    .With(s => s.StartUtc).EqualsWhen((a, b) => Equals(a.StartUtc, b.StartUtc))
                    .ShouldEqual(_result);

            private static bool Equals(DateTime a, DateTime b) =>
                a.Date == b.Date
                && Equals(a.TimeOfDay, b.TimeOfDay);

            private static bool Equals(TimeSpan a, TimeSpan b) =>
                a.Hours == b.Hours
                && a.Minutes == b.Minutes
                && a.Seconds == b.Seconds;
        }
    }
}