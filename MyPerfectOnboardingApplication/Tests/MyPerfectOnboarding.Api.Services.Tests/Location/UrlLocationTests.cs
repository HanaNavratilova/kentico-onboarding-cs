using System;
using System.Web.Http.Routing;
using System.Web.Routing;
using MyPerfectOnboarding.Api.Services.Location;
using MyPerfectOnboarding.Contracts.Services.Location;
using NSubstitute;
using NUnit.Framework;

namespace MyPerfectOnboarding.Api.Services.Tests.Location
{
    [TestFixture]
    public class UrlLocationTests
    {
        [Test]
        public void GetListItemLocation_ItemId_ReturnsCorrectlyCreatedLocation()
        {
            var id = new Guid("0B9E6EAF-83DC-4A99-9D57-A39FAF258CAC");
            var expectedUri = new Uri($"http://www.aaa.com/try/to/create/uri/with/id/{id}/and/it/is/awesome");
            var urlLocator = GetUrlLocator(id, expectedUri);

            var uri = urlLocator.GetListItemLocation(id);

            Assert.That(uri, Is.EqualTo(expectedUri));
        }

        private static UrlLocator GetUrlLocator(Guid id, Uri expectedUri)
        {
            const string routeName = "aaaaa";

            var urlHelper = Substitute.For<UrlHelper>();
            urlHelper
                .Link(
                    routeName,
                    Arg.Is<object>(routeValues => IsGivenIdInRouteValues(routeValues, id)))
                        .Returns(expectedUri.ToString()
                );

            var urlLocatorConfig = Substitute.For<IControllersRouteNames>();
            urlLocatorConfig
                .ListItemRouteName
                .Returns(routeName);

            return new UrlLocator(urlHelper, urlLocatorConfig);
        }

        private static bool IsGivenIdInRouteValues(object obj, Guid expectedId)
        {
            // ReSharper disable once CollectionNeverUpdated.Local
            var dictionary = new RouteValueDictionary(obj);
            var id = (Guid)dictionary["id"];

            return id == expectedId;
        }
    }
}
