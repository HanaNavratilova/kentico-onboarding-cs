using System.Web.Http;
using MyPerfectOnboarding.Api.Configuration;
using MyPerfectOnboarding.Api.Services;
using MyPerfectOnboarding.Contracts;
using MyPerfectOnboarding.Contracts.Services.Location;
using MyPerfectOnboarding.Database;
using Unity;
using Unity.Lifetime;

namespace MyPerfectOnboarding.Api
{
    public static class ContainerConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var container = new UnityContainer();
            RegisterTypes(container);

            config.DependencyResolver = new DependencyResolver(container);
        }

        internal static void RegisterTypes(IUnityContainer container)
            => container
                .RegisterTypesFrom<DatabaseBootstraper>()
                .RegisterTypesFrom<ServicesBootstraper>()
                .RegisterType<IControllersRouteNames, ControllersRouteNames>(new HierarchicalLifetimeManager());

        internal static IUnityContainer RegisterTypesFrom<TBootstraper>(this IUnityContainer container)
            where TBootstraper : IBootstraper, new()
            => new TBootstraper().RegisterTypesTo(container);
    }
}