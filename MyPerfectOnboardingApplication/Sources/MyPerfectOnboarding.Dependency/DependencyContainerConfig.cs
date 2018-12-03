using System.Web.Http;
using MyPerfectOnboarding.Api.Services;
using MyPerfectOnboarding.Contracts.Dependency;
using MyPerfectOnboarding.Contracts.Services.Database;
using MyPerfectOnboarding.Contracts.Services.Location;
using MyPerfectOnboarding.Database;
using MyPerfectOnboarding.Dependency.Containers;
using MyPerfectOnboarding.Dependency.DependencyResolvers;
using MyPerfectOnboarding.Services;
using Unity;

namespace MyPerfectOnboarding.Dependency
{
    public class DependencyContainerConfig
    {
        private readonly IControllersRouteNames _routeNames;
        private readonly IConnectionDetails _connectionDetails;

        public DependencyContainerConfig(IControllersRouteNames routeNames, IConnectionDetails connectionDetails)
        {
            _routeNames = routeNames;
            _connectionDetails = connectionDetails;
        }

        internal void RegisterTypes(IContainer container)
            => container
                .RegisterBootstraper<DatabaseBootstraper>()
                .RegisterBootstraper<ApiServicesBootstraper>()
                .RegisterBootstraper<ServicesBootstraper>()
                .RegisterInstance(_connectionDetails)
                .RegisterInstance(_routeNames);

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
