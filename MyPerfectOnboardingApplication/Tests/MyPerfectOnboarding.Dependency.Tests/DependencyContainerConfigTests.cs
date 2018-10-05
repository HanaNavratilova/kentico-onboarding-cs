using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using MyPerfectOnboarding.Api.Configuration;
using MyPerfectOnboarding.Contracts;
using MyPerfectOnboarding.Contracts.Dependency;
using NSubstitute;
using NUnit.Framework;
using Unity;

namespace MyPerfectOnboarding.Dependency.Tests
{
    [TestFixture]
    class DependencyContainerConfigTests
    {
        private static readonly IEnumerable<Type> TypesNotToRegister = new[]
        {
            typeof(IBootstraper),
            typeof(IContainer)
        };

        private static readonly IEnumerable<Type> TypesToRegisterExplicitly = new[]
        {
            typeof(HttpRequestMessage),
            typeof(IUnityContainer)
        };

        [Test]
        public void RegisterTypes_UnityContainer_RegisterAllBootstrapers()
        {
            var expectedTypes = GetExpectedTypes();
            var routeNames = new ControllersRouteNames();
            var dependencyContainer = new DependencyContainerConfig(routeNames);
            var config = Substitute.For<HttpConfiguration>();

            dependencyContainer.Register(config);
            if (dependencyContainer.Container is Container container) { 
                var unityContainer = container.UnityContainer;
                var registeredTypes = unityContainer.Registrations.Select(r => r.RegisteredType).ToArray();

                var missingTypes = registeredTypes.Except(expectedTypes).ToHashSet();
                var unwantedTypes = expectedTypes.Except(registeredTypes).ToHashSet();

                Assert.That(unwantedTypes, Is.Empty, "Following types were registered but they should not be:");
                Assert.That(missingTypes, Is.Empty, "Following types should be registered:");
            }
            Assert.That(dependencyContainer.Container, Is.Not.Null, "Container is null");
        }

        private static HashSet<Type> GetExpectedTypes()
        {
            var interfacesInContracts =
                typeof(IBootstraper)
                    .Assembly
                    .GetTypes()
                    .Where(x => x.IsInterface)
                    .ToArray();

            return Enumerable.Empty<Type>()
                .Union(TypesToRegisterExplicitly)
                .Union(interfacesInContracts)
                .Except(TypesNotToRegister)
                .ToHashSet();
        }
    }
}
