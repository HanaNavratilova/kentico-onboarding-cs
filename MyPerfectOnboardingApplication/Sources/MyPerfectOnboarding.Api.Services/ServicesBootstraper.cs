using System.Net.Http;
using System.Web;
using MyPerfectOnboarding.Contracts;
using MyPerfectOnboarding.Contracts.Services.Location;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace MyPerfectOnboarding.Api.Services.Location
{
    public class ServicesBootstraper : IBootstraper
    {
        public IUnityContainer RegisterTypesTo(IUnityContainer container)
            => container
                .RegisterType<IUrlLocator, UrlLocator>(new HierarchicalLifetimeManager())
                .RegisterType<HttpRequestMessage>(new InjectionFactory(unityContainer => HttpContext.Current.Items["MS_HttpRequestMessage"]));
    }
}
