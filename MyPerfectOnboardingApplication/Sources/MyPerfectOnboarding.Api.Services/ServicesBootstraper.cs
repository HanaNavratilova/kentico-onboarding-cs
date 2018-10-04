using System;
using System.Net.Http;
using System.Web;
using MyPerfectOnboarding.Api.Services.Location;
using MyPerfectOnboarding.Contracts;
using MyPerfectOnboarding.Contracts.Services.Location;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace MyPerfectOnboarding.Api.Services
{
    public class ServicesBootstraper : IBootstraper
    {
        public IUnityContainer RegisterTypesTo(IUnityContainer container)
            => container
                .RegisterType<IUrlLocator, UrlLocator>(new HierarchicalLifetimeManager())
                .RegisterType<HttpRequestMessage>(new HierarchicalLifetimeManager(), new InjectionFactory(_ => GetMessage()));

        private static HttpRequestMessage GetMessage()
            => HttpContext.Current.Items["MS_HttpRequestMessage"] as HttpRequestMessage;
    }
}
