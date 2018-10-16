using System.Web.Http;
using MyPerfectOnboarding.Api.Services;
using MyPerfectOnboarding.Contracts.Dependency;
using MyPerfectOnboarding.Contracts.Services.Location;
using MyPerfectOnboarding.Database;
using MyPerfectOnboarding.Dependency.Containers;
using MyPerfectOnboarding.Dependency.DependencyResolvers;
using MyPerfectOnboarding.Dependency.Extensions;
using Unity;

namespace MyPerfectOnboarding.Dependency
{
    public class DependencyContainerConfig
    {
        private readonly IControllersRouteNames _routeNames;

        public DependencyContainerConfig(IControllersRouteNames routeNames)
        {
            _routeNames = routeNames;
        }

        internal void RegisterTypes(IContainer container)
            => container
                .RegisterBootstraper<DatabaseBootstraper>()
                .RegisterBootstraper<ServicesBootstraper>()
                .Register(_routeNames)
                .Register(container);

        public void Register(HttpConfiguration config)
        {
            var unityContainer = new UnityContainer();
            var container = new Container(unityContainer);
            RegisterTypes(container);
            var dependencyResolver = new DependencyResolver(unityContainer);
            config.DependencyResolver = dependencyResolver;
        }
    }
}
