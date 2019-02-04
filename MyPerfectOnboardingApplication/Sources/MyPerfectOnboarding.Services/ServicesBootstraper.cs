using MyPerfectOnboarding.Contracts;
using MyPerfectOnboarding.Contracts.Dependency;
using MyPerfectOnboarding.Contracts.Services.Generators;
using MyPerfectOnboarding.Contracts.Services.ListItem;
using MyPerfectOnboarding.Services.Generators;
using MyPerfectOnboarding.Services.Services;

namespace MyPerfectOnboarding.Services
{
    public class ServicesBootstraper : IBootstraper
    {
        public IContainer RegisterTypesTo(IContainer container)
            => container
                .Register<IEditingService, EditingService>(Lifetime.PerRequest)
                .Register<IAdditionService, AdditionService>(Lifetime.PerRequest)
                .Register<IListCache, ListCache>(Lifetime.PerApplication)
                .Register<IGuidGenerator, GuidGenerator>(Lifetime.PerApplication)
                .Register<ITimeGenerator, TimeGenerator>(Lifetime.PerApplication);
    }
}
