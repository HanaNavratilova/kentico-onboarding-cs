using System;
using System.Linq;
using MyPerfectOnboarding.Contracts.Services.Generators;
using MyPerfectOnboarding.Services.Generators;
using NUnit.Framework;

namespace MyPerfectOnboarding.Services.Tests.Generators
{
    [TestFixture]
    internal class GuidGeneratorTests
    {
        private IGuidGenerator _guidGenerator;

        [SetUp]
        public void Init()
        {
            _guidGenerator = new GuidGenerator();
        }

        [Test]
        public void Generate_ReturnsGuid()
        {
            const int numberOfGeneratedGuids = 20;
            var listOfIds = Enumerable
                .Repeat<Func<Guid>>(_guidGenerator.Generate, numberOfGeneratedGuids)
                .Select(generator => generator())
                .ToArray();
            var setOfIds = listOfIds.ToHashSet();

            Assert.That(listOfIds.Count, Is.EqualTo(setOfIds.Count));
        }

        [Test]
        public void Generate_ReturnsNonemptyGuid()
        {
            var guid = _guidGenerator.Generate();

            Assert.That(guid, Is.Not.EqualTo(Guid.Empty));
        }
    }
}
