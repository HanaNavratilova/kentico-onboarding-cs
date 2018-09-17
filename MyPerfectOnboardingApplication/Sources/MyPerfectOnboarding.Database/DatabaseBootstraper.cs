using MyPerfectOnboarding.Contracts;
using MyPerfectOnboarding.Contracts.Database;
using Unity;
using Unity.Lifetime;

namespace MyPerfectOnboarding.Database
{
    public class DatabaseBootstraper : IBootstraper
    {
        public IUnityContainer RegisterTypesTo(IUnityContainer container)
            => container.RegisterType<IListRepository, ListRepository>(new HierarchicalLifetimeManager());
    }
}
