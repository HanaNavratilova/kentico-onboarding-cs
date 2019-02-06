using System;
using System.Linq;
using System.Threading;
using MyPerfectOnboarding.Contracts.Services.Generators;
using MyPerfectOnboarding.Services.Generators;
using NUnit.Framework;

namespace MyPerfectOnboarding.Services.Tests.Generators
{
    [TestFixture]
    internal class TimeGeneratorTests
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
            var lengthOfSleep = TimeSpan.FromMilliseconds(milliseconds);

            var time1 = _timeGenerator.GetCurrentTime();
            Thread.Sleep(lengthOfSleep);
            var time2 = _timeGenerator.GetCurrentTime();

            var countedLengthOfSleep = time2 - time1;
            Assert.That(countedLengthOfSleep, Is.EqualTo(lengthOfSleep).Within(milliseconds/2).Milliseconds);
        }

        [Test]
        public void GetCurrentTime_FollowingTimeIsBiggerThanPreviousOne()
        {
            const int numberOfGeneratedTimes = 20;
            var listOfTimes = Enumerable
                .Repeat<Func<DateTime>>(_timeGenerator.GetCurrentTime, numberOfGeneratedTimes)
                .Select(generator =>
                {
                    Thread.Sleep(10);
                    return generator();
                })
                .ToArray();

            var setOfTimes = listOfTimes.ToHashSet();

            
            Assert.Multiple(() => {
                Assert.That(listOfTimes.Count, Is.EqualTo(setOfTimes.Count));
                Assert.That(listOfTimes, Is.Ordered.Ascending);
            });
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