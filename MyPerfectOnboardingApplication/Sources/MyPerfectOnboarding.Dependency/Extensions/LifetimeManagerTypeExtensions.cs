using MyPerfectOnboarding.Contracts.Dependency;
using Unity.Lifetime;

namespace MyPerfectOnboarding.Dependency.Extensions
{
    internal static class LifetimeManagerTypeExtensions
    {
        public static LifetimeManager CreateLifetimeManager(this LifetimeManagerType lifetimeManagerType)
        {
            switch (lifetimeManagerType)
            {
                case LifetimeManagerType.HierarchicalLifetimeManager:
                    return new HierarchicalLifetimeManager();
                case LifetimeManagerType.ContainerControlledLifetimeManager:
                    return new ContainerControlledLifetimeManager();
                default:
                    return null;
            }
        }
    }
}
