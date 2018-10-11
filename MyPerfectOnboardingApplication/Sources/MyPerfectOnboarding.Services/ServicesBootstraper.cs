using MyPerfectOnboarding.Contracts;
using MyPerfectOnboarding.Contracts.Dependency;
using MyPerfectOnboarding.Contracts.Services.Database.Generators;
using MyPerfectOnboarding.Contracts.Services.Database.Services;
using MyPerfectOnboarding.Services.Generators;
using MyPerfectOnboarding.Services.Services;

namespace MyPerfectOnboarding.Services
{
    public class ServicesBootstraper : IBootstraper
    {
        public IContainer RegisterTypesTo(IContainer container)
            => container
                .RegisterType<IPutService, PutService>(LifetimeManagerType.HierarchicalLifetimeManager)
                .RegisterType<IPostService, PostService>(LifetimeManagerType.HierarchicalLifetimeManager)
                .RegisterType<IListCache, ListCache>(LifetimeManagerType.ContainerControlledLifetimeManager)
                .RegisterType<IGuidGenerator, GuidGenerator>(LifetimeManagerType.ContainerControlledLifetimeManager)
                .RegisterType<ITimeGenerator, TimeGenerator>(LifetimeManagerType.ContainerControlledLifetimeManager);
    }
}
