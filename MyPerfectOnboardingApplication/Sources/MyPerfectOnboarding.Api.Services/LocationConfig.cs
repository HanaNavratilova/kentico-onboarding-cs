using MyPerfectOnboarding.Contracts;
using Unity;
using Unity.Lifetime;

namespace MyPerfectOnboarding.Api.Services
{
    public class LocationConfig : IBootstraper
    {
        public IUnityContainer RegisterTypesTo(IUnityContainer container)
            => container.RegisterType <IUrlLocation, UrlLocation> (new HierarchicalLifetimeManager());
    }
}
