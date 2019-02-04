using System;
using System.Threading.Tasks;
using MyPerfectOnboarding.Contracts.Models;
using MyPerfectOnboarding.Contracts.Services.Generators;
using MyPerfectOnboarding.Contracts.Services.ListItem;
using MyPerfectOnboarding.Services.Services;
using MyPerfectOnboarding.Tests.Utils.Comparers;
using NSubstitute;
using NUnit.Framework;

namespace MyPerfectOnboarding.Services.Tests.Services
{
    [TestFixture]
    class EditingServiceTests
    {
        private IListCache _listCache;
        private ITimeGenerator _timeGenerator;
        private EditingService _editingService;

        [SetUp]
        public void Init()
        {
            _listCache = Substitute.For<IListCache>();
            _timeGenerator = Substitute.For<ITimeGenerator>();

            _editingService = new EditingService(_listCache, _timeGenerator);
        }

        [Test]
        public async Task ReplaceItemAsync_item_CallsUpdateItem()
        {
            var id = new Guid("0B9E6EAF-83DC-4A99-9D57-A39FAF258CAC");
            var item = new ListItem
            {
                Id = id,
                Text = "aaaaa",
                IsActive = false,
                CreationTime = new DateTime(1589, 12, 3),
                LastUpdateTime = new DateTime(1589, 12, 3)
            };
            
            var editedItem = new ListItem
            {
                Id = id,
                Text = "newText",
                IsActive = true,
                CreationTime = new DateTime(1845, 12, 3),
                LastUpdateTime = new DateTime(1845, 12, 3)
            };

            var updateTime = new DateTime(1896, 4, 7);

            var expectedItem = new ListItem
            {
                Id = item.Id,
                Text = editedItem.Text,
                IsActive = editedItem.IsActive,
                CreationTime = item.CreationTime,
                LastUpdateTime = updateTime
            };

            _listCache.GetItemAsync(id).Returns(item);
            _timeGenerator.GetCurrentTime().Returns(updateTime, DateTime.MinValue);

            await _editingService.ReplaceItemAsync(editedItem);

            await _listCache.Received(1).ReplaceItemAsync(Arg.Is<ListItem>(listItem => ListItemEqualityComparer.Instance.Equals(listItem, expectedItem)));
        }
    }
}
