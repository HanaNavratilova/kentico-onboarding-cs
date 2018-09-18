using System.Web.Http;
using MyPerfectOnboarding.Api.Services;
using MyPerfectOnboarding.Contracts;
using MyPerfectOnboarding.Database;
using Unity;

namespace MyPerfectOnboarding.Api
{
    public static class ContainerConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var container = new UnityContainer()
                .RegisterTypesFrom<DatabaseBootstraper>()
                .RegisterTypesFrom<ServicesBootstraper>();

            config.DependencyResolver = new DependencyResolver(container);
        }

        internal static IUnityContainer RegisterTypesFrom<T>(this IUnityContainer container)
            where T : IBootstraper, new()
            => new T().RegisterTypesTo(container);
    }
}