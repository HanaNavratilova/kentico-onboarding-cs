using System.Web.Http;
using MyPerfectOnboarding.Api.Services.Location;
using MyPerfectOnboarding.Contracts;
using MyPerfectOnboarding.Contracts.Api;
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
        {
            container
                .RegisterTypesFrom<DatabaseBootstraper>()
                .RegisterTypesFrom<ServicesBootstraper>()
                .RegisterType<IUrlLocatorConfig, UrlLocatorConfig>(new HierarchicalLifetimeManager());
        }

        internal static IUnityContainer RegisterTypesFrom<T>(this IUnityContainer container)
            where T : IBootstraper, new()
            => new T().RegisterTypesTo(container);
    }
}