using System;
using System.Threading.Tasks;
using MyPerfectOnboarding.Contracts.Models;
using MyPerfectOnboarding.Contracts.Services.Database.Generators;
using MyPerfectOnboarding.Contracts.Services.Database.Services;
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
            var item = new ListItem
            {
                Id = Guid.Empty,
                Text = "aaaaa",
                IsActive = false,
                CreationTime = new DateTime(1589, 12, 3),
                LastUpdateTime = new DateTime(1589, 12, 3)
            };
            var id = new Guid("0B9E6EAF-83DC-4A99-9D57-A39FAF258CAC");
            var time = new DateTime(2150, 12, 5);
            _timeGenerator.GetCurrentTime().Returns(time);
            _guidGenerator.Generate().Returns(id);
            _listCache.GetItemAsync(id).Returns(new ListItem(), new ListItem(), null);
            var expectedItem = new ListItem
            {
                Id = id,
                Text = item.Text,
                IsActive = false,
                CreationTime = time,
                LastUpdateTime = time
            };

            
            await _postService.AddItemAsync(item);

            await _listCache.Received(3).GetItemAsync(Arg.Any<Guid>());
            await _listCache.Received(1).AddItemAsync(Arg.Is<ListItem>(listItem => ListItemEqualityComparer.Instance.Equals(listItem, expectedItem)));
        }
    }
}
