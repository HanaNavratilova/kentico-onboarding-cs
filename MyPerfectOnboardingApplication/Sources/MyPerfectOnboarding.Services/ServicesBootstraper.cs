using MyPerfectOnboarding.Contracts.Dependency;
using MyPerfectOnboarding.Contracts.Services.Generators;
using MyPerfectOnboarding.Services.Generators;

namespace MyPerfectOnboarding.Services
{
    public class ServicesBootstraper : IBootstraper
    {
        public IContainer RegisterTypesTo(IContainer container)
            => container
                .Register<IGuidGenerator, GuidGenerator>(Lifetime.PerApplication)
                .Register<ITimeGenerator, TimeGenerator>(Lifetime.PerApplication);
    }
}
