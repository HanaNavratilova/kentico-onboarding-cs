using System;
using System.Net.Http;
using System.Web;
using MyPerfectOnboarding.Api.Services.Location;
using MyPerfectOnboarding.Contracts.Dependency;
using MyPerfectOnboarding.Contracts.Services.Location;

namespace MyPerfectOnboarding.Api.Services
{
    public class ServicesBootstraper : IBootstraper
    {
        public IContainer RegisterTypesTo(IContainer container)
            => container
                .RegisterType<IUrlLocator, UrlLocator>()
                .RegisterType(GetMessage);

        private static HttpRequestMessage GetMessage()
            => HttpContext.Current.Items["MS_HttpRequestMessage"] as HttpRequestMessage;
    }
}
