using System;
using System.Web.Http;
using MyPerfectOnboarding.Api.Services;
using MyPerfectOnboarding.Contracts.Dependency;
using MyPerfectOnboarding.Contracts.Services.Location;
using MyPerfectOnboarding.Database;
using Unity;

namespace MyPerfectOnboarding.Dependency
{
    public class DependencyContainerConfig
    {
        internal readonly IContainer Container;
        private readonly IControllersRouteNames _routeNames;

        public DependencyContainerConfig(IControllersRouteNames routeNames)
        {
            Container = new Container(new UnityContainer());
            _routeNames = routeNames;
        }

        internal void RegisterTypes(IContainer container)
            => container
                .RegisterBootstraper<DatabaseBootstraper>()
                .RegisterBootstraper<ServicesBootstraper>()
                .RegisterType(() => _routeNames);

        public void Register(HttpConfiguration config)
        {
            RegisterTypes(Container);
            var dependencyResolver = new DependencyResolver(Container);
            config.DependencyResolver = dependencyResolver;
        }
    }
}
