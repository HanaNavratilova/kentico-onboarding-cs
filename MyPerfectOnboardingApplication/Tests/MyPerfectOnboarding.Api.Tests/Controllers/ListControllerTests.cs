using MyPerfectOnboarding.Api.Controllers;
using MyPerfectOnboarding.Contracts.Models;
using MyPerfectOnboarding.Contracts.Services.Location;
using MyPerfectOnboarding.Tests.Utils.Extensions;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using MyPerfectOnboarding.Contracts.Services.ListItems;
using MyPerfectOnboarding.Tests.Utils.Comparers;

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
        private IEditingService _editingService;
        private IUrlLocator _location;
        private IListCache _cache;

        [SetUp]
        public void Init()
        {
            _location = Substitute.For<IUrlLocator>();
            _additionService = Substitute.For<IAdditionService>();
            _editingService = Substitute.For<IEditingService>();
            _cache = Substitute.For<IListCache>();

            _listController = new ListController(_location, _additionService, _editingService, _cache)
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };
        }

        [Test]
        public async Task Get_ReturnsListOfItems()
        {
            _cache.GetAllItemsAsync().Returns(_items);

            var message = await _listController.ExecuteAction(controller => controller.GetAsync());
            message.TryGetContentValue(out IEnumerable<ListItem> items);

            Assert.Multiple(() => { 
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(items, Is.EqualTo(_items));
            });
        }

        [Test]
        public async Task Get_ExistingId_ReturnsItemWithGivenId()
        {
            var expectedItem = _items[0];
            _cache.ExistsItemWithIdAsync(expectedItem.Id).Returns(true);
            _cache.GetItemAsync(expectedItem.Id).Returns(Task.FromResult(expectedItem));

            var message = await _listController.ExecuteAction(controller => controller.GetAsync(expectedItem.Id));
            message.TryGetContentValue(out ListItem item);

            Assert.Multiple(() => {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(item, Is.EqualTo(expectedItem));
            });
        }

        [Test]
        public async Task Get_EmptyId_BadRequest()
        {
            var id = Guid.Empty;

            var message = await _listController.ExecuteAction(controller => controller.GetAsync(id));
            message.TryGetContentValue(out HttpError error);

            Assert.Multiple(() => {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(error.ModelState, Does.ContainKey(nameof(ListItem.Id)));
            });
        }

        [Test]
        public async Task Get_NonexistentId_ReturnsNotFound()
        {
            var id = new Guid("22AC59B7-9517-4EDD-9DDD-EB418A7C1678");
            _cache.ExistsItemWithIdAsync(id).Returns(false);

            var message = await _listController.ExecuteAction(controller => controller.GetAsync(id));

            Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task Post_ItemOnlyWithNonemptyText_CreatedReturned()
        {
            var createdItem = _items[0];
            var newItem = new ListItem { Text = createdItem.Text };
            var expectedUri = new Uri($"http://www.aaa.com/{createdItem.Id}");
            _additionService.AddItemAsync(newItem).Returns(createdItem);
            _location.GetListItemLocation(createdItem.Id).Returns(expectedUri);

            var message = await _listController.ExecuteAction(controller => controller.PostAsync(newItem));
            message.TryGetContentValue(out ListItem item);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.Created));
                Assert.That(message.Headers.Location, Is.EqualTo(expectedUri));
                Assert.That(item, Is.EqualTo(createdItem));
            });
        }

        [Test]
        public async Task Post_ItemOnlyWithEmptyText_BadRequestReturned()
        {
            var newItem = new ListItem { Text = string.Empty};

            var message = await _listController.ExecuteAction(controller => controller.PostAsync(newItem));
            message.TryGetContentValue(out HttpError error);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(error.ModelState, Does.ContainKey(nameof(ListItem.Text)));
            });
        }

        [Test]
        public async Task Post_ItemWithNonemptyTextAndId_BadRequestReturned()
        {
            var newItem = new ListItem { Id = new Guid("22AC59B7-9517-4EDD-9DDD-EB418A7C1689"), Text = "aaa" };

            var message = await _listController.ExecuteAction(controller => controller.PostAsync(newItem));
            message.TryGetContentValue(out HttpError error);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(error.ModelState, Does.ContainKey(nameof(ListItem.Id)));
            });
        }


        [Test]
        public async Task Post_ItemWithNonemptyTextAndCreationTime_BadRequestReturned()
        {
            var newItem = new ListItem { Text = "aaa", CreationTime = new DateTime(2014,12,31)};

            var message = await _listController.ExecuteAction(controller => controller.PostAsync(newItem));
            message.TryGetContentValue(out HttpError error);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(error.ModelState, Does.ContainKey(nameof(ListItem.CreationTime)));
            });
        }

        [Test]
        public async Task Post_ItemWithNonemptyTextAndCreationTimeAndId_BadRequestReturned()
        {
            var newItem = new ListItem { Id = new Guid("22AC59B7-9517-4EDD-9DDD-EB418A7C1689"), Text = "aaa", CreationTime = new DateTime(2014, 12, 31) };

            var message = await _listController.ExecuteAction(controller => controller.PostAsync(newItem));
            message.TryGetContentValue(out HttpError error);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(error.ModelState, Does.ContainKey(nameof(ListItem.Id)));
                Assert.That(error.ModelState, Does.ContainKey(nameof(ListItem.CreationTime)));
            });
        }

        [Test]
        public async Task Post_ItemWithNonemptyTextAndCreationTimeAnLastUpdateTime_BadRequestReturned()
        {
            var newItem = new ListItem {Text = "aaa", CreationTime = new DateTime(2014, 12, 31), LastUpdateTime = new DateTime(2018, 11, 25) };

            var message = await _listController.ExecuteAction(controller => controller.PostAsync(newItem));
            message.TryGetContentValue(out HttpError error);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(error.ModelState, Does.ContainKey(nameof(ListItem.LastUpdateTime)));
                Assert.That(error.ModelState, Does.ContainKey(nameof(ListItem.CreationTime)));
            });
        }

        [Test]
        public async Task Post_ItemWithEmptyText_BadRequestReturned()
        {
            var newItem = new ListItem { Text = String.Empty };

            var message = await _listController.ExecuteAction(controller => controller.PostAsync(newItem));
            message.TryGetContentValue(out HttpError error);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(error.ModelState, Does.ContainKey(nameof(ListItem.Text)));
            });
        }

        [Test]
        public async Task Post_ItemWithEmptyTextAndId_BadRequestReturned()
        {
            var newItem = new ListItem { Id = new Guid("22AC59B7-9517-4EDD-9DDD-EB418A7C1689"), Text = String.Empty };

            var message = await _listController.ExecuteAction(controller => controller.PostAsync(newItem));
            message.TryGetContentValue(out HttpError error);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(error.ModelState, Does.ContainKey(nameof(ListItem.Text)));
                Assert.That(error.ModelState, Does.ContainKey(nameof(ListItem.Id)));
            });
        }

        [Test]
        public async Task Put_Id_OkReturned()
        {
            var id = new Guid("22AC59B7-9517-4EDD-9DDD-EB418A7C1689");
            var item = new ListItem { Text = "newItem" };
            var expectedItem = new ListItem { Id = new Guid("41CCB9DF-F43E-4C72-9F51-1C5C96D3DA84"), Text = "item"};
            _cache.ExistsItemWithIdAsync(id).Returns(true);
            _editingService.ReplaceItemAsync(id, item).Returns(expectedItem);

            var message = await _listController.ExecuteAction(controller => controller.PutAsync(id, item));
            message.TryGetContentValue(out ListItem returnedItem);

            Assert.Multiple(async () =>
            {
                await _editingService.Received().ReplaceItemAsync(id, item);
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(returnedItem, Is.EqualTo(expectedItem));
            });
        }

        [Test]
        public async Task Put_InvalidId_BadRequestReturned()
        {
            var id = Guid.Empty;
            var item = new ListItem { Text = "newItem" };

            var message = await _listController.ExecuteAction(controller => ((ListController)controller).PutAsync(id, item));
            message.TryGetContentValue(out ListItem resultItem);

            Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task Put_NonexistingIdItem_CreatedReturned()
        {
            var id = new Guid("22AC59B7-9517-4EDD-9DDD-EB418A7C1689");
            var item = new ListItem { Text = "newItem" };
            var createdItem = _items[0];
            _cache.ExistsItemWithIdAsync(id).Returns(false);
            var expectedUri = new Uri($"http://www.aaa.com/{createdItem.Id}");
            _additionService.AddItemAsync(item).Returns(createdItem);
            _location.GetListItemLocation(createdItem.Id).Returns(expectedUri);

            var message = await _listController.ExecuteAction(controller => controller.PutAsync(id, item));
            message.TryGetContentValue(out ListItem returnedItem);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.Created));
                Assert.That(message.Headers.Location, Is.EqualTo(expectedUri));
                Assert.That(returnedItem, Is.EqualTo(createdItem));
            });
        }

        [Test]
        public async Task Put_NullItem_BadRequest()
        {
            var message = await _listController.ExecuteAction(controller => controller.PutAsync(_items[0].Id, null));
            message.TryGetContentValue(out HttpError error);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(error.ModelState, Does.ContainKey(nameof(ListItem)));
            });
        }

        [Test]
        public async Task Put_ItemOnlyWithEmptyText_BadRequestReturned()
        {
            var item = new ListItem { Text = string.Empty };

            var message = await _listController.ExecuteAction(controller => controller.PutAsync(_items[0].Id, item));
            message.TryGetContentValue(out HttpError error);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(error.ModelState, Does.ContainKey(nameof(ListItem.Text)));
            });
        }

        [Test]
        public async Task Put_ItemWithNonemptyTextAndId_BadRequestReturned()
        {
            var item = new ListItem { Id = new Guid("22AC59B7-9517-4EDD-9DDD-EB418A7C1689"), Text = "aaa" };

            var message = await _listController.ExecuteAction(controller => controller.PutAsync(item.Id, item));
            message.TryGetContentValue(out HttpError error);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(error.ModelState, Does.ContainKey(nameof(ListItem.Id)));
            });
        }


        [Test]
        public async Task Put_ItemWithNonemptyTextAndCreationTime_BadRequestReturned()
        {
            var item = new ListItem { Text = "aaa", CreationTime = new DateTime(2014, 12, 31) };

            var message = await _listController.ExecuteAction(controller => controller.PutAsync(_items[0].Id, item));
            message.TryGetContentValue(out HttpError error);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(error.ModelState, Does.ContainKey(nameof(ListItem.CreationTime)));
            });
        }

        [Test]
        public async Task Put_ItemWithNonemptyTextAndCreationTimeAndId_BadRequestReturned()
        {
            var item = new ListItem { Id = new Guid("22AC59B7-9517-4EDD-9DDD-EB418A7C1689"), Text = "aaa", CreationTime = new DateTime(2014, 12, 31) };

            var message = await _listController.ExecuteAction(controller => controller.PutAsync(_items[0].Id, item));
            message.TryGetContentValue(out HttpError error);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(error.ModelState, Does.ContainKey(nameof(ListItem.Id)));
                Assert.That(error.ModelState, Does.ContainKey(nameof(ListItem.CreationTime)));
            });
        }

        [Test]
        public async Task Put_ItemWithNonemptyTextAndCreationTimeAnLastUpdateTime_BadRequestReturned()
        {
            var item = new ListItem { Text = "aaa", CreationTime = new DateTime(2014, 12, 31), LastUpdateTime = new DateTime(2018, 11, 25) };

            var message = await _listController.ExecuteAction(controller => controller.PutAsync(_items[0].Id, item));
            message.TryGetContentValue(out HttpError error);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(error.ModelState, Does.ContainKey(nameof(ListItem.LastUpdateTime)));
                Assert.That(error.ModelState, Does.ContainKey(nameof(ListItem.CreationTime)));
            });
        }

        [Test]
        public async Task Put_ItemWithEmptyTextAndId_BadRequestReturned()
        {
            var item = new ListItem { Id = _items[0].Id, Text = String.Empty };

            var message = await _listController.ExecuteAction(controller => controller.PutAsync(item.Id, item));
            message.TryGetContentValue(out HttpError error);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(error.ModelState, Does.ContainKey(nameof(ListItem.Text)));
                Assert.That(error.ModelState, Does.ContainKey(nameof(ListItem.Id)));
            });
        }

        [Test]
        public async Task Delete_Id_NoContentReturned()
        {
            var id = new Guid("22AC59B7-9517-4EDD-9DDD-EB418A7C1678");
            _cache.ExistsItemWithIdAsync(id).Returns(true);

            var message = await _listController.ExecuteAction(controller => controller.DeleteAsync(id));

            await _cache.Received().DeleteItemAsync(id);
            Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test]
        public async Task Delete_InvalidId_BadRequestReturned()
        {
            var id = Guid.Empty;

            var message = await _listController.ExecuteAction(controller => controller.DeleteAsync(id));
            message.TryGetContentValue(out HttpError error);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(error.ModelState, Does.ContainKey(nameof(ListItem.Id)));
            });
        }

        [Test]
        public async Task Delete_NonexistingId_NotFoundReturned()
        {
            var id = new Guid("22AC59B7-9517-4EDD-9DDD-EB418A7C1678");
            _cache.ExistsItemWithIdAsync(id).Returns(false);

            var message = await _listController.ExecuteAction(controller => controller.DeleteAsync(id));

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
                _cache.DidNotReceive().DeleteItemAsync(Arg.Any<Guid>());
            });
        }
    }
}