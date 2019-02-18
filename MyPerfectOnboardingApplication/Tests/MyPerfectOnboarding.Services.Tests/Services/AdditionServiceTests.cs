using System;
using System.Threading.Tasks;
using MyPerfectOnboarding.Contracts.Models;
using MyPerfectOnboarding.Contracts.Services.Generators;
using MyPerfectOnboarding.Contracts.Services.ListItems;
using MyPerfectOnboarding.Services.Services;
using MyPerfectOnboarding.Tests.Utils.Extensions;
using NSubstitute;
using NUnit.Framework;

namespace MyPerfectOnboarding.Services.Tests.Services
{
    [TestFixture]
    internal class AdditionServiceTests
    {
        private IListCache _listCache;
        private ITimeGenerator _timeGenerator;
        private IGuidGenerator _guidGenerator;
        private AdditionService _additionService;

        [SetUp]
        public void Init()
        {
            _listCache = Substitute.For<IListCache>();
            _timeGenerator = Substitute.For<ITimeGenerator>();
            _guidGenerator = Substitute.For<IGuidGenerator>();

            _additionService = new AdditionService(_listCache, _timeGenerator, _guidGenerator);
        }

        [Test]
        public async Task AddItemAsync_item_AddItemIntoRepo()
        {
            var item = new ListItem
            {
                Id = new Guid("D19AC027-B913-4F55-8F21-869A784AEB29"),
                Text = "aaaaa",
                IsActive = true,
                CreationTime = new DateTime(1589, 12, 3),
                LastUpdateTime = new DateTime(1589, 12, 3)
            };
            var id = new Guid("0B9E6EAF-83DC-4A99-9D57-A39FAF258CAC");
            var time = new DateTime(2150, 12, 5);
            var expectedItem = new ListItem
            {
                Id = id,
                Text = "aaaaa",
                IsActive = false,
                CreationTime = time,
                LastUpdateTime = time
            };
            _timeGenerator.GetCurrentTime().Returns(time, DateTime.MinValue);
            _guidGenerator.Generate().Returns(expectedItem.Id);
            _listCache.GetItemAsync(expectedItem.Id).Returns(new ListItem(), new ListItem(), null, new ListItem());
           

            await _additionService.AddItemAsync(item);

            _guidGenerator.Received(3).Generate();
            await _listCache.Received(1).AddItemAsync(ArgExtended.IsListItem(expectedItem));
        }
    }
}
