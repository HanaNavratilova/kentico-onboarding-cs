using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using MyPerfectOnboarding.Contracts;
using NUnit.Framework;
using Unity;

namespace MyPerfectOnboarding.Api.Tests
{
    [TestFixture]
    internal class ContainerConfigTests
    {
        private static readonly IEnumerable<Type> TypesNotToRegister = new[]
        {
            typeof(IBootstraper)
        };

        private static readonly IEnumerable<Type> TypesToRegisterExplicitly = new[]
        {
            typeof(HttpRequestMessage),
            typeof(IUnityContainer)
        };

        [Test]
        public void RegisterTypes_UnityContainer_RegisterAllBootstrapers()
        {
            var container = new UnityContainer();
            var expectedTypes = GetExpectedTypes();

            ContainerConfig.RegisterTypes(container);
            var registeredTypes = container.Registrations.Select(r => r.RegisteredType).ToArray();

            var missingTypes = registeredTypes.Except(expectedTypes).ToHashSet();
            var unwantedTypes = expectedTypes.Except(registeredTypes).ToHashSet();

            Assert.That(unwantedTypes, Is.Empty, "Following types were registered but they should not be:");
            Assert.That(missingTypes, Is.Empty, "Following types should be registered:");
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
