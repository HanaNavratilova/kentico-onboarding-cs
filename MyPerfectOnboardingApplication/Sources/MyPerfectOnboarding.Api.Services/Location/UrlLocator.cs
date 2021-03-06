﻿using System;
using System.Web.Http.Routing;
using MyPerfectOnboarding.Contracts.Services.Location;

namespace MyPerfectOnboarding.Api.Services.Location
{
    internal class UrlLocator : IUrlLocator
    {
        private readonly IControllersRouteNames _urlLocatorConfig;
        private readonly UrlHelper _urlHelper;

        public UrlLocator(UrlHelper url, IControllersRouteNames config)
        {
            _urlHelper = url;
            _urlLocatorConfig = config;
        }

        public Uri GetListItemLocation(Guid id)
            => new Uri(_urlHelper.Link(_urlLocatorConfig.ListItemRouteName, new { id }));
    }
}
