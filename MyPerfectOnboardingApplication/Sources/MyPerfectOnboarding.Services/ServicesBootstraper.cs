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
                .RegisterType<IPutService, PutService>(Lifetime.PerRequest)
                .RegisterType<IPostService, PostService>(Lifetime.PerRequest)
                .RegisterType<IListCache, ListCache>(Lifetime.PerApplication)
                .RegisterType<IGuidGenerator, GuidGenerator>(Lifetime.PerApplication)
                .RegisterType<ITimeGenerator, TimeGenerator>(Lifetime.PerApplication);
    }
}
