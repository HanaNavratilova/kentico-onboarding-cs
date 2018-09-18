using System;
using System.Web.Http.Routing;
using MyPerfectOnboarding.Contracts.Api;
using MyPerfectOnboarding.Contracts.Services.Location;

namespace MyPerfectOnboarding.Api.Services.Location
{
    internal class UrlLocator : IUrlLocator
    {
        private readonly IUrlLocatorConfig _urlLocatorConfig;
        private readonly UrlHelper _urlHelper;

        public UrlLocator(UrlHelper url, IUrlLocatorConfig config)
        {
            _urlHelper = url;
            _urlLocatorConfig = config;
        }

        public Uri GetListItemLocation(Guid id)
            => new Uri(_urlHelper.Link(_urlLocatorConfig.ListItemRouteName, new { id }));
    }
}
