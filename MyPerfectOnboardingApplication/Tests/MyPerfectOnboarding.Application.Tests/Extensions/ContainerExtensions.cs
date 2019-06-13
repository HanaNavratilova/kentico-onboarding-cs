using MyPerfectOnboarding.Dependency.Containers;
using NSubstitute;

namespace MyPerfectOnboarding.Application.Tests.Extensions
{
    internal static class ContainerExtensions
    {
        internal static TContract RegisterMock<TContract>(this Container container)
            where TContract : class
        {
            var mock = Substitute.For<TContract>();
            container.Register(mock);
            return mock;
        }
    }
}
