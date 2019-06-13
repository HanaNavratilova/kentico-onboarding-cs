using System;
using MyPerfectOnboarding.Contracts.Dependency;
using Unity.Lifetime;

namespace MyPerfectOnboarding.Dependency.Extensions
{
    internal static class LifetimeExtensions
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
                    throw new ArgumentOutOfRangeException($"There is no unity lifetime manager of this type.");
            }
        }
    }
}
