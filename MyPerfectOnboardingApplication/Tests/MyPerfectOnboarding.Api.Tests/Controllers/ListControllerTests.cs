using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using MyPerfectOnboarding.Api.Controllers;
using MyPerfectOnboarding.Contracts.Models;
using MyPerfectOnboarding.Contracts.Services.ListItem;
using MyPerfectOnboarding.Contracts.Services.Location;
using MyPerfectOnboarding.Tests.Utils.Extensions;
using NSubstitute;
using NUnit.Framework;

namespace MyPerfectOnboarding.Api.Tests.Controllers
{
    [TestFixture]
    internal class ListControllerTests
    {
        private ListController _listController;

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

        private IAdditionService _additionService;
        private IUrlLocator _location;
        private IListCache _cache;

        [SetUp]
        public void Init()
        {
            _location = Substitute.For<IUrlLocator>();
            _additionService = Substitute.For<IAdditionService>();
            _cache = Substitute.For<IListCache>();

            _listController = new ListController(_location, _additionService, _cache)
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };
        }

        [Test]
        public async Task Get_ReturnsListOfItems()
        {
            _cache.GetAllItemsAsync().Returns(Task.FromResult(_items as IEnumerable<ListItem>));

            var message = await _listController.ExecuteAction(controller => ((ListController)controller).GetAsync());
            message.TryGetContentValue(out IEnumerable<ListItem> items);

            Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(items, Is.EqualTo(_items).UsingComparer());
        }

        [Test]
        public async Task Get_Id_ReturnsItemWithGivenId()
        {
            var expectedItem = _items[0];
            _cache.ExistsItemWithIdAsync(expectedItem.Id).Returns(true);
            _cache.GetItemAsync(expectedItem.Id).Returns(Task.FromResult(expectedItem));

            var message = await _listController.ExecuteAction(controller => ((ListController)controller).GetAsync(expectedItem.Id));
            message.TryGetContentValue(out ListItem item);

            Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(item, Is.EqualTo(expectedItem).UsingComparer());
        }

        [Test]
        public async Task Get_Id_ReturnsNotFound()
        {
            var id = new Guid("22AC59B7-9517-4EDD-9DDD-EB418A7C1678");
            _cache.ExistsItemWithIdAsync(id).Returns(false);

            var message = await _listController.ExecuteAction(controller => ((ListController)controller).GetAsync(id));

            Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task Post_CreatedReturned()
        {
            var newItem = new ListItem { Text = "newItem" };
            var createdItem = new ListItem
            {
                Id = new Guid("0B9E6EAF-83DC-4A99-9D57-A39FAF258CAB"),
                Text = "newItem",
                IsActive = false,
                CreationTime = new DateTime(1589, 12, 3),
                LastUpdateTime = new DateTime(1896, 4, 7)
            };
            var expectedUri = new Uri($"http://www.aaa.com/{createdItem.Id}");
            _additionService.AddItemAsync(newItem).Returns(createdItem);
            _location.GetListItemLocation(createdItem.Id).Returns(expectedUri);

            var message = await _listController.ExecuteAction(controller => ((ListController)controller).PostAsync(newItem));
            message.TryGetContentValue(out ListItem item);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.Created));
                Assert.That(message.Headers.Location, Is.EqualTo(expectedUri));
                Assert.That(item, Is.EqualTo(createdItem).UsingComparer());
            });
        }

        [Test]
        public async Task Put_NoContentReturned()
        {
            var id = new Guid("22AC59B7-9517-4EDD-9DDD-EB418A7C1689");
            var item = new ListItem { Text = "newItem" };

            var message = await _listController.ExecuteAction(controller => ((ListController)controller).PutAsync(id, item));

            await _cache.Received().ReplaceItemAsync(item);
            Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test]
        public async Task Delete_NoContentReturned()
        {
            var id = new Guid("22AC59B7-9517-4EDD-9DDD-EB418A7C1678");

            var message = await _listController.ExecuteAction(controller => ((ListController)controller).DeleteAsync(id));

            await _cache.Received().DeleteItemAsync(id);
            Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }
    }
}