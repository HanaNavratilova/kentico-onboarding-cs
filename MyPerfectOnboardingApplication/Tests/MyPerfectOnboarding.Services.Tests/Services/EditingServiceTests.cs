using System;
using System.Threading.Tasks;
using MyPerfectOnboarding.Contracts.Models;
using MyPerfectOnboarding.Contracts.Services.Generators;
using MyPerfectOnboarding.Contracts.Services.ListItem;
using MyPerfectOnboarding.Services.Services;
using MyPerfectOnboarding.Tests.Utils.Builders;
using MyPerfectOnboarding.Tests.Utils.Comparers;
using NSubstitute;
using NUnit.Framework;

namespace MyPerfectOnboarding.Services.Tests.Services
{
    [TestFixture]
    internal class EditingServiceTests
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
            var stringId = "0B9E6EAF-83DC-4A99-9D57-A39FAF258CAC";
            var stringUpdateTime = "1896-04-07";
            var item = ListItemBuilder.CreateItem(stringId, "aaaaa", "1589-12-03");
            var editedItem = ListItemBuilder.CreateItem(stringId, "newText", "1845-12-03", isActive: true);
            var expectedItem = ListItemBuilder.CreateItem(stringId, "newText", "1589-12-03", stringUpdateTime, true);
            var updateTime = DateTime.Parse(stringUpdateTime);


            _listCache.GetItemAsync(item.Id).Returns(item);
            _timeGenerator.GetCurrentTime().Returns(updateTime, DateTime.MinValue);

            await _editingService.ReplaceItemAsync(item.Id, editedItem);

            await _listCache.Received(1).ReplaceItemAsync(Arg.Is<ListItem>(listItem => ListItemEqualityComparer.Instance.Equals(listItem, expectedItem)));
        }
    }
}
