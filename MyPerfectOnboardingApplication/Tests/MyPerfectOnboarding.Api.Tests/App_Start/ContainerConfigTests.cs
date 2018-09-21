using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using MyPerfectOnboarding.Contracts;
using NUnit.Framework;
using Unity;

namespace MyPerfectOnboarding.Api.Tests
{
    [TestFixture]
    class ContainerConfigTests
    {
        private static readonly HashSet<Type> InterfacesNotToRegister = new HashSet<Type>
        {
            typeof(IBootstraper)
        };

        private static readonly HashSet<Type> InterfacesToRegister = new HashSet<Type>
        {
            typeof(HttpRequestMessage),
            typeof(IUnityContainer)

        };

        [Test]
        public void RegisterTypes_UnityContainer_RegisterAllBootstrapers()
        {
            var container = new UnityContainer();
            var interfacesInContracts = 
                typeof(IBootstraper)
                    .Assembly
                    .GetTypes()
                    .Where(x => x.IsInterface)
                    .ToHashSet();

            var expectedInterfaces = Enumerable.Empty<Type>()
                .Union(InterfacesToRegister)
                .Union(interfacesInContracts)
                .Except(InterfacesNotToRegister)
                .ToHashSet();

            
            ContainerConfig.RegisterTypes(container);
            var interfaces = container.Registrations.Select(r => r.RegisteredType).ToHashSet();

            Assert.That(interfaces, Is.EquivalentTo(expectedInterfaces));
        }
    }
}
