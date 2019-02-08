using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using MyPerfectOnboarding.Api.Controllers;
using MyPerfectOnboarding.Contracts.Database;
using MyPerfectOnboarding.Contracts.Models;
using MyPerfectOnboarding.Contracts.Services.Generators;
using MyPerfectOnboarding.Contracts.Services.Location;
using MyPerfectOnboarding.Dependency;
using MyPerfectOnboarding.Dependency.Containers;
using MyPerfectOnboarding.Tests.Utils.Builders;
using MyPerfectOnboarding.Tests.Utils.Extensions;
using NSubstitute;
using NUnit.Framework;
using Unity;

namespace MyPerfectOnboarding.App.Tests
{
    [TestFixture]
    internal class AppTests
    {
        private ListController _listController;
        private IUrlLocator _location;
        private ITimeGenerator _timeGenerator;
        private IGuidGenerator _guidGenerator;
        private IListRepository _listRepository;

        [SetUp]
        public void Init()
        {
            _location = Substitute.For<IUrlLocator>();
            _timeGenerator = Substitute.For<ITimeGenerator>();
            _guidGenerator = Substitute.For<IGuidGenerator>();
            _listRepository = Substitute.For<IListRepository>();

            var routeNames = Substitute.For<IControllersRouteNames>();
            var connectionDetails = Substitute.For<IConnectionDetails>();

            var unityContainer = new UnityContainer();
            var container = new Container(unityContainer);

            var dependencyContainerConfig = new DependencyContainerConfig(routeNames, connectionDetails);
            dependencyContainerConfig.RegisterTypes(container);

            container.Register(_location);
            container.Register(_timeGenerator);
            container.Register(_guidGenerator);
            container.Register(_listRepository);

            _listController = container.UnityContainer.Resolve<ListController>();
            _listController.Request = new HttpRequestMessage();
            _listController.Configuration = new HttpConfiguration();
        }

        [Test]
        public async Task GetAll()
        {
            ListItem[] items =
            {
                ListItemBuilder.CreateItem("0B9E6EAF-83DC-4A99-9D57-A39FAF258CAC", "aaaaa", "1589-12-03", "1896-04-07"),
                ListItemBuilder.CreateItem("11AC59B7-9517-4EDD-9DDD-EB418A7C1644", "dfads", "4568-06-23", "8569-08-24")
            };

            _listRepository.GetAllItemsAsync().Returns(items);

            var message = await _listController.ExecuteAction(controller => controller.GetAsync());
            message.TryGetContentValue(out IEnumerable<IListItem> resultItems);

            Assert.Multiple(() => {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(resultItems, Is.EqualTo(items));
            });
        }

        //don't worry, I will create some more tests :)
    }
}
