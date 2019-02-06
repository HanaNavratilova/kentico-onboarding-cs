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
            // all properties but Text are supposed to be ignored, hence non-default values that would normally be passed in
            var item = ListItemBuilder.CreateItem("D19AC027-B913-4F55-8F21-869A784AEB29", "aaaaa", "1589-12-03");
            var stringId = "0B9E6EAF-83DC-4A99-9D57-A39FAF258CAC";
            var stringTime = "2150-12-05";
            var expectedItem = ListItemBuilder.CreateItem(stringId, "aaaaa", stringTime);

            var id = Guid.Parse(stringId);
            var time = DateTime.Parse(stringTime);
            // IMPORTANT: second parameter ;)
            _timeGenerator.GetCurrentTime().Returns(time, DateTime.MinValue);
            _guidGenerator.Generate().Returns(id);
            _listCache.ExistsItemWithIdAsync(id).Returns(true, true, false, true);

            await _additionService.AddItemAsync(item);

            _guidGenerator.Received(3).Generate();
            //await _listCache.Received(1).AddItemAsync(item); //-ish
            await _listCache.Received(1).AddItemAsync(Arg.Is<ListItem>(listItem => ListItemEqualityComparer.Instance.Equals(listItem, expectedItem)));
            //Assert.That(result, Is.EqualTo(item));
        }
    }
}
