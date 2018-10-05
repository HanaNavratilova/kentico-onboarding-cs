using MyPerfectOnboarding.Contracts;
using MyPerfectOnboarding.Contracts.Database;
using MyPerfectOnboarding.Contracts.Dependency;
using MyPerfectOnboarding.Database.Repository;

namespace MyPerfectOnboarding.Database
{
    public class DatabaseBootstraper : IBootstraper
    {
        public IContainer RegisterTypesTo(IContainer container)
            => container.RegisterType<IListRepository, ListRepository>();
    }
}
