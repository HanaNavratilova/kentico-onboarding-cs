using System.Web.Http;
using MyPerfectOnboarding.Api.Services;
using MyPerfectOnboarding.Contracts.Database;
using MyPerfectOnboarding.Contracts.Dependency;
using MyPerfectOnboarding.Contracts.Services.Location;
using MyPerfectOnboarding.Database;
using MyPerfectOnboarding.Dependency.Containers;
using MyPerfectOnboarding.Dependency.DependencyResolvers;
using MyPerfectOnboarding.Dependency.Extensions;
using MyPerfectOnboarding.Services;
using Unity;

namespace MyPerfectOnboarding.Dependency
{
    public class DependencyContainerConfig
    {
        private readonly IControllersRouteNames _routeNames;
        private readonly IConnectionDetails _connectionDetails;

        public static DependencyContainerConfig Create<TRouteNames, TConnection>()
            where TRouteNames : IControllersRouteNames, new()
            where TConnection : IConnectionDetails, new()
            => new DependencyContainerConfig(new TRouteNames(), new TConnection());

        internal DependencyContainerConfig(IControllersRouteNames routeNames, IConnectionDetails connectionDetails)
        {
            _routeNames = routeNames;
            _connectionDetails = connectionDetails;
        }

        internal void RegisterTypes(IContainer container)
            => container
                .RegisterBootstraper<DatabaseBootstraper>()
                .RegisterBootstraper<ApiServicesBootstraper>()
                .RegisterBootstraper<ServicesBootstraper>()
                .Register(_connectionDetails)
                .Register(_routeNames)
                .Register(container);

        public DependencyContainerConfig Register(HttpConfiguration config)
        {
            var unityContainer = new UnityContainer();
            var container = new Container(unityContainer);

            RegisterTypes(container);

            var dependencyResolver = new DependencyResolver(unityContainer);
            config.DependencyResolver = dependencyResolver;

            return this;
        }
    }
}
