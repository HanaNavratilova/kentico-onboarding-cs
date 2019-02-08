using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dependencies;
using MyPerfectOnboarding.Api.Controllers;
using MyPerfectOnboarding.Application.Tests.Extensions;
using MyPerfectOnboarding.Contracts.Database;
using MyPerfectOnboarding.Contracts.Models;
using MyPerfectOnboarding.Contracts.Services.Generators;
using MyPerfectOnboarding.Contracts.Services.Location;
using MyPerfectOnboarding.Dependency;
using MyPerfectOnboarding.Dependency.Containers;
using MyPerfectOnboarding.Dependency.DependencyResolvers;
using MyPerfectOnboarding.Tests.Utils.Builders;
using MyPerfectOnboarding.Tests.Utils.Extensions;
using NSubstitute;
using NUnit.Framework;
using Unity;

namespace MyPerfectOnboarding.Application.Tests.Api.Controllers
{
    [TestFixture]
    internal class ListControllerApplicationTests
    {
        private ListController _listController;
        private IUrlLocator _location;
        private ITimeGenerator _timeGenerator;
        private IGuidGenerator _guidGenerator;
        private IListRepository _listRepository;

        ListItem[] _items =
        {
            ListItemBuilder.CreateItem("0B9E6EAF-83DC-4A99-9D57-A39FAF258CAC", "aaaaa", "1589-12-03", "1896-04-07"),
            ListItemBuilder.CreateItem("11AC59B7-9517-4EDD-9DDD-EB418A7C1644", "dfads", "4568-06-23", "8569-08-24")
        };

        [SetUp]
        public void Init()
        {
            var container = CreateContainer();
            RegisterTypes(container);
            var resolver = new DependencyResolver(container.UnityContainer);
            CreateController(resolver);
        }

        private void CreateController(IDependencyResolver resolver)
        {
            _listController = (ListController)resolver.GetService(typeof(ListController));
            _listController.Request = new HttpRequestMessage();
            _listController.Configuration = new HttpConfiguration();
        }

        private Container CreateContainer()
        {
            var unityContainer = new UnityContainer();
            return new Container(unityContainer);
        }

        public void RegisterTypes(Container container)
        {
            var routeNames = Substitute.For<IControllersRouteNames>();
            var connectionDetails = Substitute.For<IConnectionDetails>();

            var dependencyContainerConfig = new DependencyContainerConfig(routeNames, connectionDetails);
            dependencyContainerConfig.RegisterTypes(container);

            _location = container.RegisterMock<IUrlLocator>();
            _timeGenerator = container.RegisterMock<ITimeGenerator>();
            _guidGenerator = container.RegisterMock<IGuidGenerator>();
            _listRepository = container.RegisterMock<IListRepository>();
        }

        [Test]
        public async Task GetAllItems_ThreeTimes_DatabaseOnlyReadOnce()
        {
            _listRepository.GetAllItemsAsync().Returns(_items);

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

                Assert.That(resultItems1, Is.EqualTo(_items).UsingListItemComparer());
                Assert.That(resultItems2, Is.EqualTo(_items).UsingListItemComparer());
                Assert.That(resultItems3, Is.EqualTo(_items).UsingListItemComparer());

                _listRepository.Received(1).GetAllItemsAsync();
            });
        }

        [Test]
        public async Task DeleteItem_SameItemTwice_DeleteItemAndSecondTimeReturnNotFound()
        {
            var itemIdToDelete = _items[1].Id;
            _listRepository.GetAllItemsAsync().Returns(_items);

            var message1 = await _listController.ExecuteAction(controller => controller.DeleteAsync(itemIdToDelete));
            var message2 = await _listController.ExecuteAction(controller => controller.DeleteAsync(itemIdToDelete));

            Assert.Multiple(() => {
                Assert.That(message1.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
                Assert.That(message2.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

                _listRepository.Received(1).GetAllItemsAsync();
                _listRepository.Received(1).DeleteItemAsync(Arg.Is<Guid>(id => id.Equals(itemIdToDelete)));
            });
        }

        [Test]
        public async Task AddItem_GetAllItems_ReturnsAllItemsIncludedNewOne()
        {
            _listRepository.GetAllItemsAsync().Returns(_items);
            var newItem = ListItemViewModelBuilder.CreateItem("newItemText");
            var addedItem = ListItemBuilder.CreateItem("22AC59B7-9517-4EDD-9DDD-EB418A7C1678", "newItemText", "2150-12-05");
            var expectedUri = new Uri($"http://www.aaa.com/{addedItem.Id}");
            _location.GetListItemLocation(addedItem.Id).Returns(expectedUri);
            _timeGenerator.GetCurrentTime().Returns(addedItem.CreationTime, DateTime.MinValue);
            _guidGenerator.Generate().Returns(addedItem.Id);
            _listRepository.AddItemAsync(ArgExtended.IsListItem(addedItem)).Returns(addedItem);
            var newItemsList = _items.Append(addedItem);

            var message1 = await _listController.ExecuteAction(controller => controller.PostAsync(newItem));
            message1.TryGetContentValue(out IListItem resultItem);
            var message2 = await _listController.ExecuteAction(controller => controller.GetAsync());
            message2.TryGetContentValue(out IEnumerable<IListItem> resultItems);

            Assert.Multiple(() => {
                Assert.That(message1.StatusCode, Is.EqualTo(HttpStatusCode.Created));
                Assert.That(message2.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(resultItem, Is.EqualTo(addedItem).UsingListItemComparer());
                Assert.That(resultItems, Is.EquivalentTo(newItemsList).UsingListItemComparer());

                _listRepository.Received(1).GetAllItemsAsync();
            });
        }

        [Test]
        public async Task GetItem_ReplaceItem_GetItOnceMore_ReturnsEditedItem()
        {
            var idToGet = _items[1].Id;
            _listRepository.GetAllItemsAsync().Returns(_items);
            var editedItem = ListItemViewModelBuilder.CreateItem("editedItemText");
            var updateTime = new DateTime(9528, 12, 28);
            _timeGenerator.GetCurrentTime().Returns(updateTime, DateTime.MinValue);
            _guidGenerator.Generate().Returns(idToGet);
            var replacedItem = _items[1].With(item => item.Text, editedItem.Text).With(item => item.LastUpdateTime, updateTime);
            _listRepository.ReplaceItemAsync(ArgExtended.IsListItem(replacedItem)).Returns(replacedItem);

            var message1 = await _listController.ExecuteAction(controller => controller.GetAsync(idToGet));
            message1.TryGetContentValue(out IListItem resultItem1);
            var message2 = await _listController.ExecuteAction(controller => controller.PutAsync(idToGet, editedItem));
            message2.TryGetContentValue(out IListItem resultItem2);
            var message3 = await _listController.ExecuteAction(controller => controller.GetAsync(idToGet));
            message3.TryGetContentValue(out IListItem resultItem3);

            Assert.Multiple(() => {
                Assert.That(message1.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(message2.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(message3.StatusCode, Is.EqualTo(HttpStatusCode.OK));

                Assert.That(resultItem1, Is.EqualTo(_items[1]).UsingListItemComparer());
                Assert.That(resultItem2, Is.EqualTo(replacedItem).UsingListItemComparer());
                Assert.That(resultItem3, Is.EqualTo(replacedItem).UsingListItemComparer());

                _listRepository.Received(1).GetAllItemsAsync();
                _listRepository.Received(0).GetItemAsync(Arg.Any<Guid>());
                _listRepository.Received(1).ReplaceItemAsync(ArgExtended.IsListItem(replacedItem));
            });

        }

        [Test]
        public async Task DeleteItem_TryGetSameItem_DeleteItemAndReturnsNotFound()
        {
            var itemIdToDelete = _items[1].Id;
            _listRepository.GetAllItemsAsync().Returns(_items);

            var message1 = await _listController.ExecuteAction(controller => controller.DeleteAsync(itemIdToDelete));
            var message2 = await _listController.ExecuteAction(controller => controller.GetAsync(itemIdToDelete));

            Assert.Multiple(() => {
                Assert.That(message1.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
                Assert.That(message2.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

                _listRepository.Received(1).GetAllItemsAsync();
                _listRepository.Received(1).DeleteItemAsync(Arg.Is<Guid>(id => id.Equals(itemIdToDelete)));
                _listRepository.Received(0).GetItemAsync(Arg.Any<Guid>());
            });
        }

        [Test]
        public async Task ReplaceItem_GetAllItems_ReturnsAllItemsIncludedEditedItem()
        {
            var id = _items[1].Id;
            _listRepository.GetAllItemsAsync().Returns(_items);
            var editedItem = ListItemViewModelBuilder.CreateItem("editedItemText");
            var updateTime = new DateTime(9528, 12, 28);
            _timeGenerator.GetCurrentTime().Returns(updateTime, DateTime.MinValue);
            _guidGenerator.Generate().Returns(id);
            var replacedItem = _items[1].With(item => item.Text, editedItem.Text).With(item => item.LastUpdateTime, updateTime);
            var newItemsList = _items.Except(new []{ _items[1] }).Append(replacedItem);
            _listRepository.ReplaceItemAsync(ArgExtended.IsListItem(replacedItem)).Returns(replacedItem);

            var message1 = await _listController.ExecuteAction(controller => controller.PutAsync(id, editedItem));
            message1.TryGetContentValue(out IListItem resultItem1);
            var message2 = await _listController.ExecuteAction(controller => controller.GetAsync());
            message2.TryGetContentValue(out IEnumerable<IListItem> resultItems);

            Assert.Multiple(() => {
                Assert.That(message1.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(message2.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(resultItem1, Is.EqualTo(replacedItem).UsingListItemComparer());
                Assert.That(resultItems, Is.EquivalentTo(newItemsList).UsingListItemComparer());

                _listRepository.Received(1).GetAllItemsAsync();
                _listRepository.Received(1).ReplaceItemAsync(ArgExtended.IsListItem(replacedItem));
            });

        }

        [Test]
        public async Task ReplaceNonExistingItem_GetIt_GetAll_WillAddTheItemAndReturnsAllItemsIncludedNewOne()
        {
            _listRepository.GetAllItemsAsync().Returns(_items);

            var stringId = "22AC59B7-9517-4EDD-9DDD-EB418A7C1678";
            var id = Guid.Parse(stringId);
            var expectedUri = new Uri($"http://www.aaa.com/{stringId}");
            _location.GetListItemLocation(id).Returns(expectedUri);
            var newItem = ListItemViewModelBuilder.CreateItem("newItemText");
            var addedItem = ListItemBuilder.CreateItem(stringId, "newItemText", "2150-12-05");
            _timeGenerator.GetCurrentTime().Returns(addedItem.CreationTime, DateTime.MinValue);
            _guidGenerator.Generate().Returns(id);
            _listRepository.AddItemAsync(ArgExtended.IsListItem(addedItem)).Returns(addedItem);
            var newItemsList = _items.Append(addedItem);

            var message1 = await _listController.ExecuteAction(controller => controller.PutAsync(id, newItem));
            var message2 = await _listController.ExecuteAction(controller => controller.GetAsync(id));
            message2.TryGetContentValue(out IListItem resultItem);
            var message3 = await _listController.ExecuteAction(controller => controller.GetAsync());
            message3.TryGetContentValue(out IEnumerable<IListItem> resultItems);

            Assert.Multiple(() => {
                Assert.That(message1.StatusCode, Is.EqualTo(HttpStatusCode.Created));
                Assert.That(message2.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(message3.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(resultItem, Is.EqualTo(addedItem).UsingListItemComparer());
                Assert.That(resultItems, Is.EquivalentTo(newItemsList).UsingListItemComparer());

                _listRepository.Received(1).GetAllItemsAsync();
                _listRepository.Received(1).AddItemAsync(ArgExtended.IsListItem(addedItem));
            });
        }
    }
}
