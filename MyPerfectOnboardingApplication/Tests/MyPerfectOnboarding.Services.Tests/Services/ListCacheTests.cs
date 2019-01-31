using System;
using System.Threading.Tasks;
using MyPerfectOnboarding.Contracts.Database;
using MyPerfectOnboarding.Contracts.Models;
using MyPerfectOnboarding.Services.Services;
using NSubstitute;
using NUnit.Framework;

namespace MyPerfectOnboarding.Services.Tests.Services
{
    [TestFixture]
    class ListCacheTests
    {
        private readonly ListItem[] _items =
        {
            new ListItem
            {
                Id = new Guid("0B9E6EAF-83DC-4A99-9D57-A39FAF258CAC"),
                Text = "aaaaa",
                IsActive = false,
                CreationTime = new DateTime(1589, 12, 3),
                LastUpdateTime = new DateTime(1896, 4, 7)
            },
            new ListItem
            {
                Id = new Guid("11AC59B7-9517-4EDD-9DDD-EB418A7C1644"),
                Text = "dfads",
                IsActive = false,
                CreationTime = new DateTime(4568, 6, 23),
                LastUpdateTime = new DateTime(8569, 8, 24)
            },
        };

        private ListCache _listCache;
        private IListRepository _listRepository;

        [SetUp]
        public void Init()
        {
            _listRepository = Substitute.For<IListRepository>();

            _listCache = new ListCache(_listRepository);
        }

        [Test]
        public async Task AddItemAsync_item_AddItemIntoRepository()
        {
            var id = new Guid("22AC59B7-9517-4EDD-9DDD-EB418A7C1678");
            _listRepository.GetAllItemsAsync().Returns(_items);
            var newItem = new ListItem{Id = id, Text = "newItem"};
            _listRepository.AddItemAsync(newItem).Returns(newItem);

            var addedItem = await _listCache.AddItemAsync(newItem);

            Assert.That(_listCache.Items, Contains.Key(id));
            Assert.That(addedItem, Is.EqualTo(newItem));
        }

        [Test]
        public async Task DeleteItemAsync_itemId_DeleteItemFromRepository()
        {
            var id = _items[0].Id;
            _listRepository.GetAllItemsAsync().Returns(_items);
            _listRepository.DeleteItemAsync(id).Returns(_items[0]);

            var deletedItem = await _listCache.DeleteItemAsync(id);

            Assert.That(_listCache.Items.Keys, Is.Not.Contains(id));
            Assert.That(deletedItem, Is.Not.Null);
            Assert.That(deletedItem.Id, Is.EqualTo(id));
        }

        [Test]
        public async Task DeleteItemAsync_itemId_ReturnsNull()
        {
            var id = new Guid("22AC59B7-9517-4EDD-9DDD-EB418A7C1678");
            _listRepository.GetAllItemsAsync().Returns(_items);

            var deletedItem = await _listCache.DeleteItemAsync(id);

            Assert.That(deletedItem, Is.Null);
        }

        [Test]
        public async Task ReplaceItemAsync_item_ReplaceItemInRepository()
        {
            var item = new ListItem
            {
                Id = _items[0].Id,
                Text = "bbbbb",
                CreationTime = _items[0].CreationTime,
                LastUpdateTime = _items[0].LastUpdateTime,
                IsActive = _items[0].IsActive,
            };
            _listRepository.GetAllItemsAsync().Returns(_items);
            
            await _listCache.ReplaceItemAsync(item);

            await _listRepository.Received(1).ReplaceItemAsync(item);
            Assert.That(_listCache.Items, Contains.Value(item));
            Assert.That(_listCache.Items.Values, Is.Not.Contains(_items[0]));
        }

        [Test]
        public async Task ReplaceItemAsync_item_Returns()
        {
            var id = new Guid("22AC59B7-9517-4EDD-9DDD-EB418A7C1678");
            var item = new ListItem { Id = id };
            _listRepository.GetAllItemsAsync().Returns(_items);

            // one needs to understand that item does not exists
            await _listCache.ReplaceItemAsync(item);

            await _listRepository.DidNotReceive().ReplaceItemAsync(item);
        }

        [Test]
        public async Task GetAllItemsAsync_ReturnsAllItems()
        {
            _listRepository.GetAllItemsAsync().Returns(_items);

            var items = await _listCache.GetAllItemsAsync();
            var items2 = await _listCache.GetAllItemsAsync();

            await _listRepository.Received(1).GetAllItemsAsync();
            Assert.That(items, Is.EqualTo(_items));
            Assert.That(items2, Is.EqualTo(_items));
        }

        [Test]
        public async Task GetItemAsync_itemId_ReturnsItemWithGivenId()
        {
            _listRepository.GetAllItemsAsync().Returns(_items);

            var item = await _listCache.GetItemAsync(_items[0].Id);

            //in all tests:
            await _listRepository.DidNotReceive().GetItemAsync(Arg.Any<Guid>());
            Assert.That(item, Is.EqualTo(_items[0]));
        }

        [Test]
        public async Task GetItemAsync_itemId_ReturnsNull()
        {
            var id = new Guid("22AC59B7-9517-4EDD-9DDD-EB418A7C1678");
            _listRepository.GetAllItemsAsync().Returns(_items);

            var item = await _listCache.GetItemAsync(id);

            await _listRepository.DidNotReceive().GetItemAsync(id);
            //should not be null but exception
            Assert.That(item, Is.Null);
        }

        [Test]
        public void ExistsItemWithId_itemId_ReturnsTrue()
        {
            _listRepository.GetAllItemsAsync().Returns(_items);

            var result = _listCache.ExistsItemWithId(_items[0].Id);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task ExistsItemWithId_itemId_ReturnsFalse()
        {
            var id = new Guid("22AC59B7-9517-4EDD-9DDD-EB418A7C1678");
            _listRepository.GetAllItemsAsync().Returns(_items);

            var result = _listCache.ExistsItemWithId(id);

            Assert.That(result, Is.False);
            // In reasonable amount of other tests:
            //Assert.That(_listCache.Items, Is.EqualTo(_items));
            // or
            await _listRepository.Received(1).GetAllItemsAsync();
        }
    }
}
