using MyPerfectOnboarding.Database;
using Unity;

namespace MyPerfectOnboarding.Api
{
    public static class ContainerConfig
    {
        public static void Register(System.Web.Http.HttpConfiguration config)
        {
            var container = new UnityContainer();

            DatabaseConfig.Register(container);
            config.DependencyResolver = new DependencyResolver(container);
        }
    }
}