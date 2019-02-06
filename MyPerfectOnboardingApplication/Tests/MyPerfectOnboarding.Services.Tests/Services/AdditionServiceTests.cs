using System;
using System.Threading.Tasks;
using MyPerfectOnboarding.Contracts.Services.Generators;
using MyPerfectOnboarding.Contracts.Services.ListItems;
using MyPerfectOnboarding.Services.Services;
using MyPerfectOnboarding.Tests.Utils.Builders;
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
            var item = ListItemBuilder.CreateItem("D19AC027-B913-4F55-8F21-869A784AEB29", "aaaaa", "1589-12-03");
            var expectedItem = ListItemBuilder.CreateItem("0B9E6EAF-83DC-4A99-9D57-A39FAF258CAC", "aaaaa", "2150-12-05");
            _timeGenerator.GetCurrentTime().Returns(expectedItem.CreationTime, DateTime.MinValue);
            _guidGenerator.Generate().Returns(expectedItem.Id);
            _listCache.ExistsItemWithIdAsync(expectedItem.Id).Returns(true, true, false, true);

            await _additionService.AddItemAsync(item);

            _guidGenerator.Received(3).Generate();
            await _listCache.Received(1).AddItemAsync(ArgExtended.IsListItem(expectedItem));
        }
    }
}
