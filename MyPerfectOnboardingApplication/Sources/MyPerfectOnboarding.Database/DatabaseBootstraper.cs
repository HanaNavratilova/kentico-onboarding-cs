using MyPerfectOnboarding.Contracts;
using MyPerfectOnboarding.Contracts.Database;
using MyPerfectOnboarding.Database.Repository;
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
