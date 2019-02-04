using System.Net.Http;
using System.Web;
using MyPerfectOnboarding.Api.Services.Location;
using MyPerfectOnboarding.Contracts;
using MyPerfectOnboarding.Contracts.Dependency;
using MyPerfectOnboarding.Contracts.Services.Location;

namespace MyPerfectOnboarding.Api.Services
{
    public class ApiServicesBootstraper : IBootstraper
    {
        public IContainer RegisterTypesTo(IContainer container)
            => container
                .Register<IUrlLocator, UrlLocator>(Lifetime.PerRequest)
                .Register(GetMessage, Lifetime.PerRequest);

        private static HttpRequestMessage GetMessage()
            => HttpContext.Current.Items["MS_HttpRequestMessage"] as HttpRequestMessage;
    }
}
