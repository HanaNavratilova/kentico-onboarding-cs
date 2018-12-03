using MyPerfectOnboarding.Contracts.Dependency;
using Unity.Lifetime;

namespace MyPerfectOnboarding.Dependency.Extensions
{
    internal static class LifetimeManagerTypeExtensions
    {
        public static LifetimeManager CreateLifetimeManager(this Lifetime lifetimeManagerType)
        {
            switch (lifetimeManagerType)
            {
                case Lifetime.PerRequest:
                    return new HierarchicalLifetimeManager();
                case Lifetime.PerApplication:
                    return new ContainerControlledLifetimeManager();
                default:
                    return null;
            }
        }
    }
}
