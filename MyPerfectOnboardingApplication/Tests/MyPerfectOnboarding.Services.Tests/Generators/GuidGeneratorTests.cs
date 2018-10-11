using System;
using System.Linq;
using MyPerfectOnboarding.Contracts.Services.Database.Generators;
using MyPerfectOnboarding.Services.Generators;
using NUnit.Framework;

namespace MyPerfectOnboarding.Services.Tests.Generators
{
    [TestFixture]
    class GuidGeneratorTests
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
            var listOfGuids = new Guid[20].Select(_ => _guidGenerator.Generate()).ToArray();
            var setOfGuids = listOfGuids.ToHashSet();

            Assert.That(listOfGuids.Count(), Is.EqualTo(setOfGuids.Count));
        }
    }
}
