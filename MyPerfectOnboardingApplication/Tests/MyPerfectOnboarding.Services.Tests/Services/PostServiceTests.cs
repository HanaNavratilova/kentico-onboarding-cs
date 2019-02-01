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
    class PostServiceTests
    {
        private IListCache _listCache;
        private ITimeGenerator _timeGenerator;
        private IGuidGenerator _guidGenerator;
        private PostService _postService;

        [SetUp]
        public void Init()
        {
            _listCache = Substitute.For<IListCache>();
            _timeGenerator = Substitute.For<ITimeGenerator>();
            _guidGenerator = Substitute.For<IGuidGenerator>();

            _postService = new PostService(_listCache, _timeGenerator, _guidGenerator);
        }

        [Test]
        public async Task AddItemAsync_item_AddItemIntoRepo()
        {
            // all properties but Text are supposed to be ignored, hence non-default values that would normally be passed in
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
            // IMPORTANT: second parameter ;)
            _timeGenerator.GetCurrentTime().Returns(time, DateTime.MinValue);
            _guidGenerator.Generate().Returns(id);
            _listCache.GetItemAsync(id).Returns(new ListItem(), new ListItem(), null, new ListItem());

            
            var result = await _postService.AddItemAsync(item);

            _guidGenerator.Received(3).Generate();
            await _listCache.Received(1).AddItemAsync(item); //-ish
            //await _listCache.Received(1).AddItemAsync(Arg.Is<ListItem>(listItem => ListItemEqualityComparer.Instance.Equals(listItem, expectedItem)));
            //Assert.That(result, Is.EqualTo(item));
        }
    }
}
