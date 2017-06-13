using System;
using Jobbie.Domain.Models;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoRhinoMock;
using Ploeh.SemanticComparison.Fluent;

namespace Jobbie.Domain.Tests.Unit.Models
{
    [TestFixture]
    internal sealed class ScheduleBuilderUnitTests
    {
        private TestContext _context;

        [SetUp]
        public void SetUp() => _context = new TestContext();

        [Test]
        public void Build_default_scheduler() =>
            _context
                .Act()
                .Assert();

        [Test]
        public void Build_scheduler_with_NextUtc() =>
            _context
                .ArrangeWithNextUtc()
                .Act()
                .Assert();

        [Test]
        public void Build_scheduler_with_PreviousUtc() =>
            _context
                .ArrangeWithPreviousUtc()
                .Act()
                .Assert();

        [Test]
        public void Build_scheduler_with_Cron() =>
            _context
                .ArrangeWithCron()
                .Act()
                .Assert();

        [Test]
        public void Build_scheduler_with_EndUtc() =>
            _context
                .ArrangeWithEndUtc()
                .Act()
                .Assert();

        private sealed class TestContext
        {
            private readonly IFixture _fixture;
            private readonly Schedule.Builder _sut;
            private Schedule _result;

            public TestContext()
            {
                _fixture = new Fixture().Customize(new AutoRhinoMockCustomization());

                _sut = _fixture.Create<Schedule.Builder>();
            }

            public TestContext ArrangeWithNextUtc()
            {
                _sut.WithNextUtc(_fixture.Create<DateTime>());
                return this;
            }

            public TestContext ArrangeWithPreviousUtc()
            {
                _sut.WithPreviousUtc(_fixture.Create<DateTime>());
                return this;
            }

            public TestContext ArrangeWithCron()
            {
                _sut.WithCron(_fixture.Create<string>());
                return this;
            }

            public TestContext ArrangeWithEndUtc()
            {
                _sut.WithEndUtc(_fixture.Create<DateTime>());
                return this;
            }

            public TestContext Act()
            {
                _result = _sut.Build();
                return this;
            }

            public void Assert() =>
                _sut
                    .AsSource()
                    .OfLikeness<Schedule>()
                    .ShouldEqual(_result);
        }
    }
}