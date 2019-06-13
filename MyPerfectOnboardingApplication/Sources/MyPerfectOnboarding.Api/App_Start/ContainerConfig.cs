using System.Web.Http;
using MyPerfectOnboarding.Api.Configuration;
using MyPerfectOnboarding.Dependency;

namespace MyPerfectOnboarding.Api
{
    public static class ContainerConfig
    {
        public static void Register(HttpConfiguration config)
            => DependencyContainerConfig
                .Create<ControllersRouteNames, ConnectionDetails>()
                .Register(config);
    }
}