using System;
using System.Collections.Generic;
using System.Linq;
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
using MyPerfectOnboarding.Tests.Utils.Comparers;
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
        public async Task GetAllItemsThreeTimes()
        {
            ListItem[] items =
            {
                ListItemBuilder.CreateItem("0B9E6EAF-83DC-4A99-9D57-A39FAF258CAC", "aaaaa", "1589-12-03", "1896-04-07"),
                ListItemBuilder.CreateItem("11AC59B7-9517-4EDD-9DDD-EB418A7C1644", "dfads", "4568-06-23", "8569-08-24")
            };

            _listRepository.GetAllItemsAsync().Returns(items);

            var message1 = await _listController.ExecuteAction(controller => controller.GetAsync());
            message1.TryGetContentValue(out IEnumerable<IListItem> resultItems1);

            var message2 = await _listController.ExecuteAction(controller => controller.GetAsync());
            message2.TryGetContentValue(out IEnumerable<IListItem> resultItems2);

            var message3 = await _listController.ExecuteAction(controller => controller.GetAsync());
            message3.TryGetContentValue(out IEnumerable<IListItem> resultItems3);

            Assert.Multiple(() => {
                Assert.That(message1.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(message2.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(message3.StatusCode, Is.EqualTo(HttpStatusCode.OK));

                Assert.That(resultItems1, Is.EqualTo(items).Using(ListItemEqualityComparer.Instance));
                Assert.That(resultItems2, Is.EqualTo(items).Using(ListItemEqualityComparer.Instance));
                Assert.That(resultItems3, Is.EqualTo(items).Using(ListItemEqualityComparer.Instance));

                _listRepository.Received(1).GetAllItemsAsync();
            });
        }

        [Test]
        public async Task TryDeleteSameItemTwice()
        {
            var stringId = "11AC59B7-9517-4EDD-9DDD-EB418A7C1644";
            var itemIdToDelete = Guid.Parse(stringId);
            ListItem[] items =
            {
                ListItemBuilder.CreateItem("0B9E6EAF-83DC-4A99-9D57-A39FAF258CAC", "aaaaa", "1589-12-03", "1896-04-07"),
                ListItemBuilder.CreateItem(stringId, "dfads", "4568-06-23", "8569-08-24")
            };

            _listRepository.GetAllItemsAsync().Returns(items);

            var message1 = await _listController.ExecuteAction(controller => controller.DeleteAsync(itemIdToDelete));
            var message2 = await _listController.ExecuteAction(controller => controller.DeleteAsync(itemIdToDelete));

            Assert.Multiple(() => {
                Assert.That(message1.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
                Assert.That(message2.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
                _listRepository.Received(1).DeleteItemAsync(Arg.Is<Guid>(id => id.Equals(itemIdToDelete)));
                _listRepository.Received(0).GetItemAsync(Arg.Is<Guid>(id => id.Equals(itemIdToDelete)));
                _listRepository.Received(1).GetAllItemsAsync();
            });
        }

        [Test]
        public async Task TryAddItemAndGetAllItems()
        {
            ListItem[] items =
            {
                ListItemBuilder.CreateItem("0B9E6EAF-83DC-4A99-9D57-A39FAF258CAC", "aaaaa", "1589-12-03", "1896-04-07"),
                ListItemBuilder.CreateItem("11AC59B7-9517-4EDD-9DDD-EB418A7C1644", "dfads", "4568-06-23", "8569-08-24")
            };
            _listRepository.GetAllItemsAsync().Returns(items);

            var stringId = "22AC59B7-9517-4EDD-9DDD-EB418A7C1678";
            var id = Guid.Parse(stringId);
            var expectedUri = new Uri($"http://www.aaa.com/{stringId}");
            _location.GetListItemLocation(id).Returns(expectedUri);

            var stringTime = "2150-12-05";
            var time = DateTime.Parse(stringTime);

            _timeGenerator.GetCurrentTime().Returns(time, DateTime.MinValue);
            _guidGenerator.Generate().Returns(id);

            var newItemText = "newItemText";
            var newItem = ListItemViewModelBuilder.CreateItem(newItemText);
            var addedItem = ListItemBuilder.CreateItem(stringId, newItemText, stringTime);
            _listRepository.AddItemAsync(Arg.Is<ListItem>(item => ListItemEqualityComparer.Instance.Equals(item, addedItem))).Returns(addedItem);
            var newItemsList = items.Append(addedItem);

            var message1 = await _listController.ExecuteAction(controller => controller.PostAsync(newItem));
            var message2 = await _listController.ExecuteAction(controller => controller.GetAsync());
            message2.TryGetContentValue(out IEnumerable<IListItem> resultItems);

            Assert.Multiple(() => {
                Assert.That(message1.StatusCode, Is.EqualTo(HttpStatusCode.Created));
                Assert.That(message2.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(resultItems, Is.EquivalentTo(newItemsList).Using(ListItemEqualityComparer.Instance));

                _listRepository.Received(1).GetAllItemsAsync();
            });

        }

        [Test]
        public async Task GetItemReplaceItAndGetItOnceMore()
        {
            var idToGet = "11AC59B7-9517-4EDD-9DDD-EB418A7C1644";
            var id = Guid.Parse(idToGet);
            ListItem[] items =
            {
                ListItemBuilder.CreateItem("0B9E6EAF-83DC-4A99-9D57-A39FAF258CAC", "aaaaa", "1589-12-03", "1896-04-07"),
                ListItemBuilder.CreateItem(idToGet, "dfads", "4568-06-23", "8569-08-24")
            };
            _listRepository.GetAllItemsAsync().Returns(items);

            var newText = "editedItemText";
            var editedItem = ListItemViewModelBuilder.CreateItem(newText);
            var time = new DateTime(9528, 12, 28);
            _timeGenerator.GetCurrentTime().Returns(time, DateTime.MinValue);
            _guidGenerator.Generate().Returns(id);
            var replacedItem = items[1].With(item => item.Text, newText).With(item => item.LastUpdateTime, time);

            var message1 = await _listController.ExecuteAction(controller => controller.GetAsync(id));
            message1.TryGetContentValue(out IListItem resultItem1);
            var message2 = await _listController.ExecuteAction(controller => controller.PutAsync(id, editedItem));
            var message3 = await _listController.ExecuteAction(controller => controller.GetAsync(id));
            message3.TryGetContentValue(out IListItem resultItem2);

            Assert.Multiple(() => {
                Assert.That(message1.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(message2.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
                Assert.That(message3.StatusCode, Is.EqualTo(HttpStatusCode.OK));

                Assert.That(resultItem1, Is.EqualTo(items[1]).Using(ListItemEqualityComparer.Instance));
                Assert.That(resultItem2, Is.EqualTo(replacedItem).Using(ListItemEqualityComparer.Instance));

                _listRepository.Received(1).GetAllItemsAsync();
                _listRepository.Received(0).GetItemAsync(Arg.Any<Guid>());
                _listRepository.Received(1).ReplaceItemAsync(Arg.Is<ListItem>(item => ListItemEqualityComparer.Instance.Equals(item, replacedItem)));
            });

        }

        [Test]
        public async Task DeleteItemAndTryToGetIt()
        {
            var stringId = "11AC59B7-9517-4EDD-9DDD-EB418A7C1644";
            var itemIdToDelete = Guid.Parse(stringId);
            ListItem[] items =
            {
                ListItemBuilder.CreateItem("0B9E6EAF-83DC-4A99-9D57-A39FAF258CAC", "aaaaa", "1589-12-03", "1896-04-07"),
                ListItemBuilder.CreateItem(stringId, "dfads", "4568-06-23", "8569-08-24")
            };

            _listRepository.GetAllItemsAsync().Returns(items);

            var message1 = await _listController.ExecuteAction(controller => controller.DeleteAsync(itemIdToDelete));
            var message2 = await _listController.ExecuteAction(controller => controller.GetAsync(itemIdToDelete));

            Assert.Multiple(() => {
                Assert.That(message1.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
                Assert.That(message2.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
                _listRepository.Received(1).DeleteItemAsync(Arg.Is<Guid>(id => id.Equals(itemIdToDelete)));
                _listRepository.Received(0).GetItemAsync(Arg.Is<Guid>(id => id.Equals(itemIdToDelete)));
                _listRepository.Received(1).GetAllItemsAsync();
            });
        }

        [Test]
        public async Task ReplaceItemAndGetAllItems()
        {
            var idToReplace = "11AC59B7-9517-4EDD-9DDD-EB418A7C1644";
            var id = Guid.Parse(idToReplace);
            ListItem[] items =
            {
                ListItemBuilder.CreateItem("0B9E6EAF-83DC-4A99-9D57-A39FAF258CAC", "aaaaa", "1589-12-03", "1896-04-07"),
                ListItemBuilder.CreateItem(idToReplace, "dfads", "4568-06-23", "8569-08-24")
            };
            _listRepository.GetAllItemsAsync().Returns(items);

            var newText = "editedItemText";
            var editedItem = ListItemViewModelBuilder.CreateItem(newText);
            var time = new DateTime(9528, 12, 28);
            _timeGenerator.GetCurrentTime().Returns(time, DateTime.MinValue);
            _guidGenerator.Generate().Returns(id);
            var replacedItem = items[1].With(item => item.Text, newText).With(item => item.LastUpdateTime, time);
            var newItemsList = items.Except(new []{ items[1] }).Append(replacedItem);

            var message1 = await _listController.ExecuteAction(controller => controller.PutAsync(id, editedItem));
            var message2 = await _listController.ExecuteAction(controller => controller.GetAsync());
            message2.TryGetContentValue(out IEnumerable<IListItem> resultItems);

            Assert.Multiple(() => {
                Assert.That(message1.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
                Assert.That(message2.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(resultItems, Is.EquivalentTo(newItemsList).Using(ListItemEqualityComparer.Instance));

                _listRepository.Received(1).GetAllItemsAsync();
                _listRepository.Received(1).ReplaceItemAsync(Arg.Is<ListItem>(item => ListItemEqualityComparer.Instance.Equals(item, replacedItem)));
            });

        }

        [Test]
        public async Task ReplaceNonExistingItemAndGetItAndGetAll_WillAddTheItem()
        {
            ListItem[] items =
            {
                ListItemBuilder.CreateItem("0B9E6EAF-83DC-4A99-9D57-A39FAF258CAC", "aaaaa", "1589-12-03", "1896-04-07"),
                ListItemBuilder.CreateItem("11AC59B7-9517-4EDD-9DDD-EB418A7C1644", "dfads", "4568-06-23", "8569-08-24")
            };
            _listRepository.GetAllItemsAsync().Returns(items);

            var stringId = "22AC59B7-9517-4EDD-9DDD-EB418A7C1678";
            var id = Guid.Parse(stringId);
            var expectedUri = new Uri($"http://www.aaa.com/{stringId}");
            _location.GetListItemLocation(id).Returns(expectedUri);

            var stringTime = "2150-12-05";
            var time = DateTime.Parse(stringTime);

            _timeGenerator.GetCurrentTime().Returns(time, DateTime.MinValue);
            _guidGenerator.Generate().Returns(id);

            var newItemText = "newItemText";
            var newItem = ListItemViewModelBuilder.CreateItem(newItemText);
            var addedItem = ListItemBuilder.CreateItem(stringId, newItemText, stringTime);
            _listRepository.AddItemAsync(Arg.Is<ListItem>(item => ListItemEqualityComparer.Instance.Equals(item, addedItem))).Returns(addedItem);
            var newItemsList = items.Append(addedItem);

            var message1 = await _listController.ExecuteAction(controller => controller.PutAsync(id, newItem));
            var message2 = await _listController.ExecuteAction(controller => controller.GetAsync(id));
            message2.TryGetContentValue(out IListItem resultItem);
            var message3 = await _listController.ExecuteAction(controller => controller.GetAsync());
            message3.TryGetContentValue(out IEnumerable<IListItem> resultItems);

            Assert.Multiple(() => {
                Assert.That(message1.StatusCode, Is.EqualTo(HttpStatusCode.Created));
                Assert.That(message2.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(message3.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(resultItem, Is.EqualTo(addedItem).Using(ListItemEqualityComparer.Instance));
                Assert.That(resultItems, Is.EquivalentTo(newItemsList).Using(ListItemEqualityComparer.Instance));

                _listRepository.Received(1).GetAllItemsAsync();
                _listRepository.Received(1).AddItemAsync(Arg.Is<ListItem>(item => ListItemEqualityComparer.Instance.Equals(item, addedItem)));
            });

        }
    }
}
