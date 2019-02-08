using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using MyPerfectOnboarding.Contracts;
using MyPerfectOnboarding.Contracts.Database;
using MyPerfectOnboarding.Contracts.Dependency;
using MyPerfectOnboarding.Contracts.Models;
using MyPerfectOnboarding.Contracts.Selector;
using MyPerfectOnboarding.Contracts.Services.Location;
using MyPerfectOnboarding.Dependency.Containers;
using NSubstitute;
using NUnit.Framework;
using Unity;

namespace MyPerfectOnboarding.Dependency.Tests
{
    [TestFixture]
    internal class DependencyContainerConfigTests
    {
        private static readonly IEnumerable<Type> TypesNotToRegister = new[]
        {
            typeof(IBootstraper),
            typeof(IListItem),
            typeof(IValueSelector<>)
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
            var routeNames = Substitute.For<IControllersRouteNames>(); ;          
            var connectionDetails = Substitute.For<IConnectionDetails>(); ;
            IUnityContainer unityContainer = new UnityContainer();
            IContainer container = new Container(unityContainer);
            var dependencyContainer = new DependencyContainerConfig(routeNames, connectionDetails);

            dependencyContainer.RegisterTypes(container);

            var registeredTypes = unityContainer.Registrations.Select(r => r.RegisteredType).ToArray();
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

            return Enumerable
                .Empty<Type>()
                .Union(TypesToRegisterExplicitly)
                .Union(interfacesInContracts)
                .Except(TypesNotToRegister)
                .ToHashSet();
        }
    }
}
