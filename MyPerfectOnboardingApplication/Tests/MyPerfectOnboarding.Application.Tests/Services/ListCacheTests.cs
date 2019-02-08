using System;
using System.Management.Instrumentation;
using System.Threading.Tasks;
using MyPerfectOnboarding.Application.Tests.Extensions;
using MyPerfectOnboarding.Contracts.Database;
using MyPerfectOnboarding.Contracts.Models;
using MyPerfectOnboarding.Contracts.Services.Generators;
using MyPerfectOnboarding.Contracts.Services.ListItems;
using MyPerfectOnboarding.Contracts.Services.Location;
using MyPerfectOnboarding.Dependency;
using MyPerfectOnboarding.Dependency.Containers;
using MyPerfectOnboarding.Services.Services;
using MyPerfectOnboarding.Tests.Utils.Builders;
using NSubstitute;
using NUnit.Framework;
using Unity;

namespace MyPerfectOnboarding.Application.Tests.Services
{
    [TestFixture]
    internal class ListCacheTests
    {
        private readonly ListItem[] _items =
        {
            ListItemBuilder.CreateItem("0B9E6EAF-83DC-4A99-9D57-A39FAF258CAC", "aaaaa", "1589-12-03", "1896-04-07"),
            ListItemBuilder.CreateItem("11AC59B7-9517-4EDD-9DDD-EB418A7C1644", "dfads", "4568-06-23", "8569-08-24")
        };

        private IListCache _listCache;
        private IListRepository _listRepository;
        private CachedItemsProvider _cachedItemsProvider;

        [SetUp]
        public void Init()
        {
            var container = CreateContainer();
            RegisterTypes(container);

            _listCache = container.UnityContainer.Resolve<IListCache>();
            _cachedItemsProvider =
                container.UnityContainer.Resolve<ICachedItemsProvider>() as CachedItemsProvider
                ?? throw new InstanceNotFoundException(
                    "Registration was not successful and in ICachedItemsProvider is not registered CachedItemsProvider.");
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

            container.RegisterMock<IUrlLocator>();
            container.RegisterMock<ITimeGenerator>();
            container.RegisterMock<IGuidGenerator>();
            _listRepository = container.RegisterMock<IListRepository>();
        }

        [Test]
        public async Task CachedItemsProvider_LoadedItemsProperlyAndExecuteOnlyOnce()
        {
            Parallel.Invoke(
                async () => await _listCache.GetAllItemsAsync(),
                async () => await _listCache.GetAllItemsAsync()
            );

            await _listRepository.Received(1).GetAllItemsAsync();
        }

        [Test]
        public async Task AddItemAsync_Item_AddItemIntoRepository()
        {
            _listRepository.GetAllItemsAsync().Returns(_items);
            var newItem = ListItemBuilder.CreateItem("22AC59B7-9517-4EDD-9DDD-EB418A7C1678", "newItem", "3240-02-29");
            _listRepository.AddItemAsync(newItem).Returns(newItem);

            var addedItem = await _listCache.AddItemAsync(newItem);

            Assert.Multiple(async () => {
                await _listRepository.Received(1).GetAllItemsAsync();
                await _listRepository.Received(1).AddItemAsync(newItem);
                Assert.That(await _cachedItemsProvider.Items, Contains.Key(newItem.Id));
                Assert.That(addedItem, Is.EqualTo(newItem));
            });
        }

        [Test]
        public async Task DeleteItemAsync_ItemId_DeleteItemFromRepository()
        {
            _listRepository.GetAllItemsAsync().Returns(_items);
            var id = _items[0].Id;
            _listRepository.DeleteItemAsync(id).Returns(_items[0]);

            var deletedItem = await _listCache.DeleteItemAsync(id);

            Assert.Multiple(async () => {
                await _listRepository.Received(1).GetAllItemsAsync();
                await _listRepository.Received(1).DeleteItemAsync(id);
                Assert.That((await _cachedItemsProvider.Items).Keys, Is.Not.Contains(id));
                Assert.That(deletedItem, Is.Not.Null.And.Property(nameof(IListItem.Id)).EqualTo(id));
            });
        }

        [Test]
        public void DeleteItemAsync_ItemId_ThrowsException()
        {
            var id = new Guid("22AC59B7-9517-4EDD-9DDD-EB418A7C1678");

            Assert.Multiple(async () => {
                Assert.ThrowsAsync<ArgumentException>(async () => await _listCache.DeleteItemAsync(id));

                await _listRepository.Received(1).GetAllItemsAsync();
                await _listRepository.DidNotReceive().DeleteItemAsync(Arg.Any<Guid>());
            });
        }

        [Test]
        public async Task ReplaceItemAsync_Item_ReplaceItemInRepository()
        {
            _listRepository.GetAllItemsAsync().Returns(_items);
            var editedItem = _items[0].With(item => item.Text, "bbbbb");

            await _listCache.ReplaceItemAsync(editedItem);

            Assert.Multiple(async () => {
                await _listRepository.Received(1).GetAllItemsAsync();
                await _listRepository.Received(1).ReplaceItemAsync(editedItem);
                Assert.That(await _cachedItemsProvider.Items, Contains.Value(editedItem));
                Assert.That(await _cachedItemsProvider.Items, Is.Not.ContainValue(_items[0]));
            });
        }

        [Test]
        public void ReplaceItemAsync_NonexistentItem_ThrowsException()
        {
            var item = ListItemBuilder.CreateItem("22AC59B7-9517-4EDD-9DDD-EB418A7C1678", "text", "1865-09-23");

            Assert.Multiple(async () => {
                Assert.ThrowsAsync<ArgumentException>(async () => await _listCache.ReplaceItemAsync(item));

                await _listRepository.Received(1).GetAllItemsAsync();
                await _listRepository.DidNotReceive().ReplaceItemAsync(Arg.Any<ListItem>());
            });
        }

        [Test]
        public async Task GetAllItemsAsync_ReturnsAllItems()
        {
            _listRepository.GetAllItemsAsync().Returns(_items);
            var firstItems = await _listCache.GetAllItemsAsync();
            var secondItems = await _listCache.GetAllItemsAsync();

            Assert.Multiple(async () => {
                await _listRepository.Received(1).GetAllItemsAsync();
                Assert.That(firstItems, Is.EqualTo(_items));
                Assert.That(secondItems, Is.EqualTo(_items));
            });
        }

        [Test]
        public async Task GetItemAsync_ItemId_ReturnsItemWithGivenId()
        {
            _listRepository.GetAllItemsAsync().Returns(_items);
            var firstItem = await _listCache.GetItemAsync(_items[0].Id);
            var secondItem = await _listCache.GetItemAsync(_items[0].Id);

            Assert.Multiple(async () => {
                await _listRepository.Received(1).GetAllItemsAsync();
                await _listRepository.DidNotReceive().GetItemAsync(Arg.Any<Guid>());
                Assert.That(firstItem, Is.EqualTo(_items[0]));
                Assert.That(secondItem, Is.EqualTo(_items[0]));
            });
        }

        [Test]
        public void GetItemAsync_ItemId_ThrowsException()
        {
            var id = new Guid("22AC59B7-9517-4EDD-9DDD-EB418A7C1678");

            Assert.Multiple(async () => {
                Assert.ThrowsAsync<ArgumentException>(async () => await _listCache.GetItemAsync(id));

                await _listRepository.Received(1).GetAllItemsAsync();
                await _listRepository.DidNotReceive().GetItemAsync(Arg.Any<Guid>());
            });
        }

        [Test]
        public async Task ExistsItemWithId_ItemId_ReturnsTrue()
        {
            _listRepository.GetAllItemsAsync().Returns(_items);
            var firstResult = await _listCache.ExistsItemWithIdAsync(_items[0].Id);
            var secondResult = await _listCache.ExistsItemWithIdAsync(_items[0].Id);

            Assert.Multiple(async () => {
                await _listRepository.Received(1).GetAllItemsAsync();
                Assert.That(firstResult, Is.True);
                Assert.That(secondResult, Is.True);
            });
        }

        [Test]
        public async Task ExistsItemWithId_ItemId_ReturnsFalse()
        {
            _listRepository.GetAllItemsAsync().Returns(_items);
            var id = new Guid("22AC59B7-9517-4EDD-9DDD-EB418A7C1678");

            var firstResult = await _listCache.ExistsItemWithIdAsync(id);
            var secondResult = await _listCache.ExistsItemWithIdAsync(id);

            Assert.Multiple(async () => {
                Assert.That(firstResult, Is.False);
                Assert.That(secondResult, Is.False);
                await _listRepository.Received(1).GetAllItemsAsync();
            });
        }
    }
}
