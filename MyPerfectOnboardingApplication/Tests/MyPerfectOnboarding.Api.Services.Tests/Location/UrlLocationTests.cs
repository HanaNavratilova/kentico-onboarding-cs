using System;
using System.Web.Http.Routing;
using System.Web.Routing;
using MyPerfectOnboarding.Api.Services.Location;
using MyPerfectOnboarding.Contracts.Api;
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
            const string name = "aaaaa";
            var urlHelper = Substitute.For<UrlHelper>();
            urlHelper.Link(name, Arg.Is<object>(obj => IsGivenIdInObject(obj, id))).Returns(expectedUri.ToString());
            var urlLocatorConfig = Substitute.For<IUrlLocatorConfig>();
            urlLocatorConfig.ListItemRouteName.Returns(name);
            var urlLocation = new UrlLocator(urlHelper, urlLocatorConfig);

            var uri = urlLocation.GetListItemLocation(id);

            Assert.That(uri, Is.EqualTo(expectedUri));
        }

        private static bool IsGivenIdInObject(object obj, Guid expectedId)
        {
            var dictionary = new RouteValueDictionary(obj);
            var id = (Guid)dictionary["id"];

            return id == expectedId;
        }
    }
}
