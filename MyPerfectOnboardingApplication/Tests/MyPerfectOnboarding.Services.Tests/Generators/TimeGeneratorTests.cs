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
            var time1 = _timeGenerator.GetCurrentTime();
            Thread.Sleep(milliseconds);
            var time2 = _timeGenerator.GetCurrentTime();

            var millisecondsAccuracy = milliseconds / 2;
            var accuracy = new TimeSpan(0, 0, 0, 0, millisecondsAccuracy);

            Assert.That(time1.AddMilliseconds(milliseconds), Is.EqualTo(time2).Within(accuracy));
        }
    }
}