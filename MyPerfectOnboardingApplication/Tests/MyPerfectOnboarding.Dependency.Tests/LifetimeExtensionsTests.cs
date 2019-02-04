using System;
using MyPerfectOnboarding.Contracts.Dependency;
using MyPerfectOnboarding.Dependency.Extensions;
using NUnit.Framework;
using Unity.Lifetime;

namespace MyPerfectOnboarding.Dependency.Tests
{
    [TestFixture]
    internal class LifetimeExtensionsTests
    {
        [Test]
        public void CreateLifetimeManager_LifetimePerRequest_ReturnsHierarchicalLifetimeManager()
        {
            var lifetime = Lifetime.PerRequest;

            var result = lifetime.CreateLifetimeManager();

            Assert.That(result, Is.InstanceOf<HierarchicalLifetimeManager>() );
        }

        [Test]
        public void CreateLifetimeManager_LifetimePerApplication_ReturnsContainerControlledLifetimeManager()
        {
            var lifetime = Lifetime.PerApplication;

            var result = lifetime.CreateLifetimeManager();

            Assert.That(result, Is.InstanceOf<ContainerControlledLifetimeManager>());
        }

        [Test]
        public void CreateLifetimeManager_UnknownLifetime_ThrowsException()
        {
            var lifetime = (Lifetime)4;

            Assert.Throws<ArgumentOutOfRangeException>(() => lifetime.CreateLifetimeManager());
        }
    }
}
