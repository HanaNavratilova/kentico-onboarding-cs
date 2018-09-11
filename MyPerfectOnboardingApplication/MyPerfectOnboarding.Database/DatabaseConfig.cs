using MyPerfectOnboarding.Contracts;
using Unity;
using Unity.Lifetime;

namespace MyPerfectOnboarding.Database
{
    public static class DatabaseConfig
    {
        public static void Register(UnityContainer container)
        {
            container.RegisterType<IListRepository, ListRepository>(new HierarchicalLifetimeManager());
        }
    }
}
