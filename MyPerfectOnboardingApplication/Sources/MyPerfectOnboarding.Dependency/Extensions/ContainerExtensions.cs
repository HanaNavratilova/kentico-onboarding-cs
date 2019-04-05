using MyPerfectOnboarding.Contracts.Dependency;

namespace MyPerfectOnboarding.Dependency.Extensions
{
    internal static class ContainerExtensions
    {

        public static IContainer RegisterBootstraper<TBootstraper>(this IContainer container)
            where TBootstraper : IBootstraper, new()
        {
            new TBootstraper().RegisterTypesTo(container);
            return container;
        }
    }
}
