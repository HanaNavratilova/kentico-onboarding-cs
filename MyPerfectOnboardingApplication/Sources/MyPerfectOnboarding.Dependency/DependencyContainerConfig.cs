using System.Web.Http;
using MyPerfectOnboarding.Api.Services;
using MyPerfectOnboarding.Contracts.Dependency;
using MyPerfectOnboarding.Contracts.Services.Database;
using MyPerfectOnboarding.Contracts.Services.Location;
using MyPerfectOnboarding.Database;
using MyPerfectOnboarding.Dependency.Containers;
using MyPerfectOnboarding.Dependency.DependencyResolvers;
using Unity;

namespace MyPerfectOnboarding.Dependency
{
    public class DependencyContainerConfig
    {
        internal readonly IContainer Container;
        private readonly IControllersRouteNames _routeNames;
        private readonly IConnectionDetails _connectionDetails;

        public DependencyContainerConfig(IControllersRouteNames routeNames, IConnectionDetails connectionDetails)
        {
            Container = new Container(new UnityContainer());
            _routeNames = routeNames;
            _connectionDetails = connectionDetails;
        }

        internal void RegisterTypes(IContainer container)
            => container
                .RegisterBootstraper<DatabaseBootstraper>()
                .RegisterBootstraper<ServicesBootstraper>()
                .RegisterInstance(_connectionDetails)
                .RegisterInstance(_routeNames);

        public void Register(HttpConfiguration config)
        {
            RegisterTypes(Container);
            var dependencyResolver = new DependencyResolver(Container);
            config.DependencyResolver = dependencyResolver;
        }
    }
}
