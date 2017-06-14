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
    internal sealed class JobSchedulerUnitTests
    {
        private TestContext _context;

        [SetUp]
        public void SetUp() => _context = new TestContext();

        [Test]
        public void Schedules_once_off_job() =>
            _context
                .ActScheduleOnceOffJob()
                .Assert();

        [Test]
        public void Schedules_never_ending_recurring_job() =>
            _context
                .ActScheduleNeverEndingRecurringJob()
                .Assert();

        [Test]
        public void Schedules_finite_recurring_job() =>
            _context
                .ActScheduleFiniteRecurringJob()
                .Assert();

        [Test]
        public void Unschedules_job() =>
            _context
                .ActUnscheduleJob()
                .Assert();

        [Test]
        public void Fails_to_schedule_once_off_job() =>
            Assert.Throws<FailedToScheduleJob>(() =>
                _context
                    .ArrangeFailedSchedulingOfJob()
                    .ActScheduleOnceOffJob());

        [Test]
        public void Fails_to_schedule_never_ending_recurring_job() =>
            Assert.Throws<FailedToScheduleJob>(() =>
                _context
                    .ArrangeFailedSchedulingOfJob()
                    .ActScheduleNeverEndingRecurringJob());

        [Test]
        public void Fails_to_schedule_finite_recurring_job() =>
            Assert.Throws<FailedToScheduleJob>(() =>
                _context
                    .ArrangeFailedSchedulingOfJob()
                    .ActScheduleFiniteRecurringJob());

        [Test]
        public void Fails_to_unschedule_job() =>
            Assert.Throws<FailedToDeleteSchedule>(() =>
                _context
                    .ArrangeFailedUnschedulingOfJob()
                    .ActUnscheduleJob());

        private sealed class TestContext
        {
            private readonly IFixture _fixture;
            private readonly IJobScheduler _sut;
            private readonly IScheduler _scheduler;
            private readonly INow _now;
            private readonly Guid _scheduleId;
            private readonly Guid _jobId;
            private readonly string _description;
            private readonly DateTime _startUtc;
            private readonly string _cron;
            private readonly DateTime _endUtc;

            public TestContext()
            {
                _fixture = new Fixture().Customize(new AutoRhinoMockCustomization());

                _scheduleId = _fixture.Create<Guid>();
                _jobId = _fixture.Create<Guid>();
                _description = _fixture.Create<string>();
                _startUtc = _fixture.Create<DateTime>();
                _cron = "* * * ? * *";
                _endUtc = _startUtc.Add(_fixture.Create<TimeSpan>());

                _scheduler = _fixture.Freeze<IScheduler>();
                _now = _fixture.Freeze<INow>();
                _sut = _fixture.Create<JobScheduler>();
            }

            public TestContext ArrangeFailedSchedulingOfJob()
            {
                _scheduler
                    .Expect(s => s.ScheduleJob(Arg<ITrigger>.Is.Anything))
                    .Throw(_fixture.Create<Exception>());
                return this;
            }

            public TestContext ArrangeFailedUnschedulingOfJob()
            {
                _scheduler
                    .Expect(s => s.UnscheduleJob(new TriggerKey(_scheduleId.ToString())))
                    .Throw(_fixture.Create<Exception>());
                return this;
            }

            public TestContext ActScheduleOnceOffJob()
            {
                _scheduler
                    .Expect(s =>
                        s.ScheduleJob(
                            Arg<ITrigger>.Matches(t =>
                                Equals(t.Key, new TriggerKey(_scheduleId.ToString()))
                                && Equals(t.JobKey, JobKey.Create(_jobId.ToString()))
                                && t.Description == _description
                                && t.StartTimeUtc == _startUtc
                                && t.JobDataMap.GetLong("CreatedUtc") == _now.Utc.Ticks)))
                    .Return(_fixture.Create<DateTimeOffset>());

                _sut.Create(_scheduleId, _jobId, _description, _startUtc);

                return this;
            }

            public TestContext ActScheduleNeverEndingRecurringJob()
            {
                _scheduler
                    .Expect(s =>
                        s.ScheduleJob(
                            Arg<ITrigger>.Matches(t =>
                                Equals(t.Key, new TriggerKey(_scheduleId.ToString()))
                                && Equals(t.JobKey, JobKey.Create(_jobId.ToString()))
                                && t.Description == _description
                                && Equals(t.StartTimeUtc, _startUtc)
                                && t.JobDataMap.GetLong("CreatedUtc") == _now.Utc.Ticks
                                && t.JobDataMap.GetString("Cron") == _cron
                                && !t.EndTimeUtc.HasValue)))
                    .Return(_fixture.Create<DateTimeOffset>());

                _sut.Create(_scheduleId, _jobId, _description, _startUtc, _cron);

                return this;
            }

            public TestContext ActScheduleFiniteRecurringJob()
            {
                _scheduler
                    .Expect(s =>
                        s.ScheduleJob(
                            Arg<ITrigger>.Matches(t =>
                                Equals(t.Key, new TriggerKey(_scheduleId.ToString()))
                                && Equals(t.JobKey, JobKey.Create(_jobId.ToString()))
                                && t.Description == _description
                                && Equals(t.StartTimeUtc, _startUtc)
                                && t.JobDataMap.GetLong("CreatedUtc") == _now.Utc.Ticks
                                && t.JobDataMap.GetString("Cron") == _cron
                                && Equals(t.EndTimeUtc.Value, _endUtc))))
                    .Return(_fixture.Create<DateTimeOffset>());

                _sut.Create(_scheduleId, _jobId, _description, _startUtc, _cron, _endUtc);

                return this;
            }

            public TestContext ActUnscheduleJob()
            {
                _scheduler
                    .Expect(s => s.UnscheduleJob(new TriggerKey(_scheduleId.ToString())))
                    .Return(true);

                _sut.Delete(_scheduleId);

                return this;
            }

            public void Assert() => _scheduler.VerifyAllExpectations();

            private static bool Equals(DateTimeOffset a, DateTime b) =>
                a.Date == b.Date
                && Equals(a.TimeOfDay, b.TimeOfDay);

            private static bool Equals(TimeSpan a, TimeSpan b) =>
                a.Hours == b.Hours
                && a.Minutes == b.Minutes
                && a.Seconds == b.Seconds;
        }
    }
}