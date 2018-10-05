using System;
using System.Web.Http;
using MyPerfectOnboarding.Api.Configuration;
using MyPerfectOnboarding.Dependency;

namespace MyPerfectOnboarding.Api
{
    public static class ContainerConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var routeNames = new ControllersRouteNames();
            var dependencyContainerConfig = new DependencyContainerConfig(routeNames);
            dependencyContainerConfig.Register(config);
        }
    }
}