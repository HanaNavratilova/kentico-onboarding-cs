using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyPerfectOnboarding.Contracts.Database;
using MyPerfectOnboarding.Contracts.Models;
using MyPerfectOnboarding.Contracts.Services.ListItems;
using MyPerfectOnboarding.Services.Services;
using MyPerfectOnboarding.Tests.Utils.Builders;
using NSubstitute;
using NUnit.Framework;

namespace MyPerfectOnboarding.Services.Tests.Services
{
    [TestFixture]
    internal class ListCacheTests
    {
        private readonly ConcurrentDictionary<Guid, ListItem> _items = new ConcurrentDictionary<Guid, ListItem>
        {
            [Guid.Parse("0B9E6EAF-83DC-4A99-9D57-A39FAF258CAC")] =
                ListItemBuilder.CreateItem("0B9E6EAF-83DC-4A99-9D57-A39FAF258CAC", "aaaaa", "1589-12-03", "1896-04-07"),
            [Guid.Parse("11AC59B7-9517-4EDD-9DDD-EB418A7C1644")] =
                ListItemBuilder.CreateItem("11AC59B7-9517-4EDD-9DDD-EB418A7C1644", "dfads", "4568-06-23", "8569-08-24")
        };

        private ListCache _listCache;
        private IListRepository _listRepository;
        private ICachedItemsProvider _cachedItemsProvider;

        [SetUp]
        public void Init()
        {
            _listRepository = Substitute.For<IListRepository>();
            _listRepository.GetAllItemsAsync().Returns(_items.Values);
            MockCachedItemsProvider();

            _listCache = new ListCache(_listRepository, _cachedItemsProvider);
        }

        private void MockCachedItemsProvider()
        {
            _cachedItemsProvider = Substitute.For<ICachedItemsProvider>();

            _cachedItemsProvider
                .ExecuteOnItemsAsync(Arg.Any<Func<ConcurrentDictionary<Guid, ListItem>, Task<ListItem>>>())
                .Returns(async callInfo => await callInfo.Arg<Func<ConcurrentDictionary<Guid, ListItem>, Task<ListItem>>>()(_items));

            MockCachedItemsProviderItemsFor<bool>();
            MockCachedItemsProviderItemsFor<ListItem>();
            MockCachedItemsProviderItemsFor<ICollection<ListItem>>();
        }

        private void MockCachedItemsProviderItemsFor<T>()
            => _cachedItemsProvider
                .ExecuteOnItems(Arg.Any<Func<ConcurrentDictionary<Guid, ListItem>, T>>())
                .Returns(callInfo => callInfo.Arg<Func<ConcurrentDictionary<Guid, ListItem>, T>>().Invoke(_items));

        [Test]
        public async Task AddItemAsync_item_AddItemIntoRepository()
        {
            var newItem = ListItemBuilder.CreateItem("22AC59B7-9517-4EDD-9DDD-EB418A7C1678", "newItem", "1475-02-20");
            _listRepository.AddItemAsync(newItem).Returns(newItem);

            var addedItem = await _listCache.AddItemAsync(newItem);

            Assert.Multiple(async () => {
                await _listRepository.Received(1).AddItemAsync(newItem);
                Assert.That(addedItem, Is.EqualTo(newItem));
            });
        }

        [Test]
        public async Task DeleteItemAsync_itemId_DeleteItemFromRepository()
        {
            var id = _items.Values.First().Id;
            _listRepository.DeleteItemAsync(id).Returns(_items[id]);

            var deletedItem = await _listCache.DeleteItemAsync(id);

            Assert.Multiple(async () => {
                await _listRepository.Received(1).DeleteItemAsync(id);
                Assert.That(deletedItem, Is.Not.Null.And.Property(nameof(ListItem.Id)).EqualTo(id));
            });
        }

        [Test]
        public void DeleteItemAsync_itemId_ThrowsException()
        {
            var id = new Guid("22AC59B7-9517-4EDD-9DDD-EB418A7C1678");

            Assert.Multiple(async () => {
                Assert.ThrowsAsync<ArgumentException>(async () => await _listCache.DeleteItemAsync(id));

                await _listRepository.DidNotReceive().DeleteItemAsync(Arg.Any<Guid>());
            });
        }

        [Test]
        public async Task ReplaceItemAsync_item_ReplaceItemInRepository()
        {
            var editedItem = _items.Values.First().With(item => item.Text, "bbbbb");

            await _listCache.ReplaceItemAsync(editedItem);

            Assert.Multiple(async () => {
                await _listRepository.Received(1).ReplaceItemAsync(editedItem);
            });
        }

        [Test]
        public void ReplaceItemAsync_NonexistentItem_ThrowsException()
        {
            var item = ListItemBuilder.CreateItem("22AC59B7-9517-4EDD-9DDD-EB418A7C1678", "text", "3158-11-25");

            Assert.Multiple(async () => {
                Assert.ThrowsAsync<ArgumentException>(async () => await _listCache.ReplaceItemAsync(item));

                await _listRepository.DidNotReceive().ReplaceItemAsync(Arg.Any<ListItem>());
            });
        }

        [Test]
        public async Task GetAllItemsAsync_ReturnsAllItems()
        {
            var items = await _listCache.GetAllItemsAsync();

            Assert.That(items, Is.EquivalentTo(_items.Values));
        }

        [Test]
        public async Task GetItemAsync_itemId_ReturnsItemWithGivenId()
        {
            var id = _items.Values.Last().Id;
            var item = await _listCache.GetItemAsync(id);

            Assert.Multiple(async () => {
                await _listRepository.DidNotReceive().GetItemAsync(Arg.Any<Guid>());
                Assert.That(item, Is.EqualTo(_items[id]));
            });
        }

        [Test]
        public void GetItemAsync_itemId_ThrowsException()
        {
            var id = new Guid("22AC59B7-9517-4EDD-9DDD-EB418A7C1678");

            Assert.Multiple(async () => {
                Assert.ThrowsAsync<ArgumentException>(async () => await _listCache.GetItemAsync(id));

                await _listRepository.DidNotReceive().GetItemAsync(Arg.Any<Guid>());
            });
        }

        [Test]
        public async Task ExistsItemWithId_itemId_ReturnsTrue()
        {
            var id = _items.Values.Last().Id;
            var result = await _listCache.ExistsItemWithIdAsync(id);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task ExistsItemWithId_itemId_ReturnsFalse()
        {
            var id = new Guid("22AC59B7-9517-4EDD-9DDD-EB418A7C1678");

            var result = await _listCache.ExistsItemWithIdAsync(id);

            Assert.That(result, Is.False);
        }
    }
}
