using System;
using System.Threading.Tasks;
using MyPerfectOnboarding.Contracts.Database;
using MyPerfectOnboarding.Contracts.Models;
using MyPerfectOnboarding.Services.Services;
using MyPerfectOnboarding.Tests.Utils.Builders;
using NSubstitute;
using NUnit.Framework;

namespace MyPerfectOnboarding.Services.Tests.Services
{
    [TestFixture]
    internal class ListCacheTests
    {
        private readonly ListItem[] _items =
        {
            ListItemBuilder.CreateItem("0B9E6EAF-83DC-4A99-9D57-A39FAF258CAC", "aaaaa", "1589-12-03", "1896-04-07"),
            ListItemBuilder.CreateItem("11AC59B7-9517-4EDD-9DDD-EB418A7C1644", "dfads", "4568-06-23", "8569-08-24")
        };

        private ListCache _listCache;
        private IListRepository _listRepository;

        [SetUp]
        public void Init()
        {
            _listRepository = Substitute.For<IListRepository>();

            _listCache = new ListCache(_listRepository);

            _listRepository.GetAllItemsAsync().Returns(_items);
        }

        [Test]
        public async Task AddItemAsync_item_AddItemIntoRepository()
        {
            var stringId = "22AC59B7-9517-4EDD-9DDD-EB418A7C1678";
            var newItem = ListItemBuilder.CreateItem(stringId, "newItem");
            var id = Guid.Parse(stringId);
            _listRepository.AddItemAsync(newItem).Returns(newItem);

            var addedItem = await _listCache.AddItemAsync(newItem);

            Assert.Multiple(async () => {
                await _listRepository.Received(1).GetAllItemsAsync();
                await _listRepository.Received(1).AddItemAsync(newItem);
                Assert.That(_listCache.Items, Contains.Key(id));
                Assert.That(addedItem, Is.EqualTo(newItem));
            });
        }

        [Test]
        public async Task DeleteItemAsync_itemId_DeleteItemFromRepository()
        {
            var id = _items[0].Id;
            _listRepository.DeleteItemAsync(id).Returns(_items[0]);

            var deletedItem = await _listCache.DeleteItemAsync(id);

            Assert.Multiple(async () => {
                await _listRepository.Received(1).GetAllItemsAsync();
                await _listRepository.Received(1).DeleteItemAsync(id);
                Assert.That(_listCache.Items.Keys, Is.Not.Contains(id));
                Assert.That(deletedItem, Is.Not.Null);
                Assert.That(deletedItem.Id, Is.EqualTo(id));
            });
        }

        [Test]
        public void DeleteItemAsync_itemId_ThrowsException()
        {
            var id = new Guid("22AC59B7-9517-4EDD-9DDD-EB418A7C1678");

            Assert.Multiple(async () => {             
                Assert.ThrowsAsync<ArgumentNullException>(async () => await _listCache.DeleteItemAsync(id));

                await _listRepository.Received(1).GetAllItemsAsync();
                await _listRepository.DidNotReceive().DeleteItemAsync(Arg.Any<Guid>());
            });
        }

        [Test]
        public async Task ReplaceItemAsync_item_ReplaceItemInRepository()
        {
            var editedItem = _items[0].With(item => item.Text, "bbbbb");
            
            await _listCache.ReplaceItemAsync(editedItem);

            Assert.Multiple(async () => {
                await _listRepository.Received(1).GetAllItemsAsync();
                await _listRepository.Received(1).ReplaceItemAsync(editedItem);
                Assert.That(_listCache.Items, Contains.Value(editedItem));
                Assert.That(_listCache.Items.Values, Is.Not.Contains(_items[0]));
            });
        }

        [Test]
        public void ReplaceItemAsync_NonexistentItem_ThrowsException()
        {
            var item = ListItemBuilder.CreateItem("22AC59B7-9517-4EDD-9DDD-EB418A7C1678");

            Assert.Multiple(async () => {
                Assert.ThrowsAsync<ArgumentNullException>(async () => await _listCache.ReplaceItemAsync(item));
                await _listRepository.Received(1).GetAllItemsAsync();
                await _listRepository.DidNotReceive().ReplaceItemAsync(Arg.Any<ListItem>());
            });
        }

        [Test]
        public async Task GetAllItemsAsync_ReturnsAllItems()
        {
            var items = await _listCache.GetAllItemsAsync();
            var items2 = await _listCache.GetAllItemsAsync();

            Assert.Multiple(async () => {
                await _listRepository.Received(1).GetAllItemsAsync();
                Assert.That(items, Is.EqualTo(_items));
                Assert.That(items2, Is.EqualTo(_items));
            });
        }

        [Test]
        public async Task GetItemAsync_itemId_ReturnsItemWithGivenId()
        {
            var item = await _listCache.GetItemAsync(_items[0].Id);

            Assert.Multiple(async () => {
                await _listRepository.Received(1).GetAllItemsAsync();
                await _listRepository.DidNotReceive().GetItemAsync(Arg.Any<Guid>());
                Assert.That(item, Is.EqualTo(_items[0]));
            });
        }

        [Test]
        public void GetItemAsync_itemId_ThrowsException()
        {
            var id = new Guid("22AC59B7-9517-4EDD-9DDD-EB418A7C1678");

            Assert.Multiple(async () => {
                Assert.ThrowsAsync<ArgumentNullException>(async () => await _listCache.GetItemAsync(id));

                await _listRepository.Received(1).GetAllItemsAsync();
                await _listRepository.DidNotReceive().GetItemAsync(Arg.Any<Guid>());
            });
        }

        [Test]
        public async Task ExistsItemWithId_itemId_ReturnsTrue()
        {
            var result = await _listCache.ExistsItemWithIdAsync(_items[0].Id);
            await _listCache.ExistsItemWithIdAsync(_items[0].Id);

            Assert.Multiple(async () => {
                await _listRepository.Received(1).GetAllItemsAsync();
                Assert.That(result, Is.True);
            });
        }

        [Test]
        public async Task ExistsItemWithId_itemId_ReturnsFalse()
        {
            var id = new Guid("22AC59B7-9517-4EDD-9DDD-EB418A7C1678");

            var result = await _listCache.ExistsItemWithIdAsync(id);

            Assert.Multiple(async () => {
                Assert.That(result, Is.False);
                await _listRepository.Received(1).GetAllItemsAsync();
            });
        }
    }
}
