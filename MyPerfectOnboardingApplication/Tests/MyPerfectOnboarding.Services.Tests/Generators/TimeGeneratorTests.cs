using System;
using System.Threading;
using MyPerfectOnboarding.Contracts.Services.Generators;
using MyPerfectOnboarding.Services.Generators;
using NUnit.Framework;

namespace MyPerfectOnboarding.Services.Tests.Generators
{
    [TestFixture]
    class TimeGeneratorTests
    {
        private ITimeGenerator _timeGenerator;

        [SetUp]
        public void Init()
        {
            _timeGenerator = new TimeGenerator();
        }

        [Test]
        public void GetCurrentTime_ReturnsTime()
        {
            const int milliseconds = 52;
            TimeSpan lenfthOfSleep = TimeSpan.FromMilliseconds(milliseconds);
            var time1 = _timeGenerator.GetCurrentTime();
            Thread.Sleep(lenfthOfSleep);
            var time2 = _timeGenerator.GetCurrentTime();

            Assert.That(time2 - time1, Is.EqualTo(lenfthOfSleep).Within(milliseconds/10).Milliseconds);
        }

        [Test]
        public void GetCurrentTime_NotReturnsMinTime()
        {
            var time = _timeGenerator.GetCurrentTime();

            Assert.That(time, Is.Not.EqualTo(DateTime.MinValue));
        }

        [Test]
        public void GetCurrentTime_NotReturnsMaxTime()
        {
            var time = _timeGenerator.GetCurrentTime();

            Assert.That(time, Is.Not.EqualTo(DateTime.MaxValue));
        }
    }
}