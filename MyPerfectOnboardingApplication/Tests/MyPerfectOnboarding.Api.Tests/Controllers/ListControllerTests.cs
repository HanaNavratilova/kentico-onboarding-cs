using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using MyPerfectOnboarding.Api.Controllers;
using MyPerfectOnboarding.Api.Tests.Utils;
using MyPerfectOnboarding.Contracts.Database;
using MyPerfectOnboarding.Contracts.Models;
using MyPerfectOnboarding.Contracts.Services.Location;
using NSubstitute;
using NUnit.Framework;

namespace MyPerfectOnboarding.Api.Tests.Controllers
{
    [TestFixture]
    class ListControllerTests
    {
        private ListController _listController;

        private readonly ListItem[] _items = {
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

        private IListRepository _repository;
        private IUrlLocator _location;

       [SetUp]
        public void Init()
        {
            _repository = Substitute.For<IListRepository>();
            _location = Substitute.For<IUrlLocator>();

            _listController = new ListController(_repository, _location)
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };
        }

        [Test]
        public async Task Get_ReturnsListOfItems()
        {
            _repository.GetAllItemsAsync().Returns(Task.FromResult(_items));

            var message = await _listController.ExecuteAction(controller => controller.GetAsync());

            message.TryGetContentValue(out IEnumerable<ListItem> items);

            Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(items, Is.EqualTo(_items).Using(new ListItemEqualityComparer()));
        }

        [Test]
        public async Task Get_ReturnsItemWithGivenId()
        {
            var expectedItem = _items[0];
            _repository.GetItemAsync(expectedItem.Id).Returns(Task.FromResult(expectedItem));

            var message = await _listController.ExecuteAction(controller => controller.GetAsync(expectedItem.Id));
            message.TryGetContentValue(out ListItem item);

            Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(item, Is.EqualTo(expectedItem).Using(new ListItemEqualityComparer()));
        }

        [Test]
        public async Task Post_CreatedReturned()
        {
            var item = new ListItem {Text = "newItem"};
            var newItem = new ListItem
            {
                Id = new Guid("0B9E6EAF-83DC-4A99-9D57-A39FAF258CAB"),
                Text = "newItem",
                IsActive = false,
                CreationTime = new DateTime(1589, 12, 3),
                LastUpdateTime = new DateTime(1896, 4, 7)
            };
            var expectedUri = new Uri($"http://www.aaa.com/{newItem.Id}");
            _repository.AddItemAsync(item).Returns(newItem);
            _location.GetListItemLocation(newItem.Id).Returns(expectedUri);

            var message = await _listController.ExecuteAction(controller => controller.PostAsync(item));

            Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(message.Headers.Location, Is.EqualTo(expectedUri));
        }

        [Test]
        public async Task Put_NoContentReturned()
        {
            var id = Guid.NewGuid();
            var item = new ListItem{Text = "newItem"};

            var message = await _listController.ExecuteAction(controller => controller.PutAsync(id, item));

            await _repository.Received().EditItemAsync(id, item);
            Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test]
        public async Task Delete_NoContentReturned()
        {
            var id = new Guid("22AC59B7-9517-4EDD-9DDD-EB418A7C1678");       

            var message = await _listController.ExecuteAction(controller => controller.DeleteAsync(id));

            await _repository.Received().DeleteItemAsync(id);
            Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }
    }
}
