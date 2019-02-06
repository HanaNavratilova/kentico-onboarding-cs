using MyPerfectOnboarding.Api.Controllers;
using MyPerfectOnboarding.Contracts.Models;
using MyPerfectOnboarding.Contracts.Services.ListItem;
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
using MyPerfectOnboarding.Api.Models;

namespace MyPerfectOnboarding.Api.Tests.Controllers
{
    [TestFixture]
    internal class ListControllerTests
    {
        private ListController _listController;

        private readonly IListItem[] _items =
        {
            new ListItemViewModel
            {
                Id = new Guid("0B9E6EAF-83DC-4A99-9D57-A39FAF258CAC"),
                Text = "aaaaa",
                IsActive = false,
                CreationTime = new DateTime(1589, 12, 3),
                LastUpdateTime = new DateTime(1896, 4, 7)
            },
            new ListItemViewModel
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
            message.TryGetContentValue(out IEnumerable<IListItem> items);

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
            _cache.GetItemAsync(expectedItem.Id).Returns(expectedItem);

            var message = await _listController.ExecuteAction(controller => controller.GetAsync(expectedItem.Id));
            message.TryGetContentValue(out IListItem item);

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
                Assert.That(error.ModelState.Keys.Contains(nameof(IListItem.Id)), Is.True);
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
            var newItem = new ListItemViewModel { Text = createdItem.Text };
            var expectedUri = new Uri($"http://www.aaa.com/{createdItem.Id}");
            _additionService.AddItemAsync(newItem).Returns(createdItem);
            _location.GetListItemLocation(createdItem.Id).Returns(expectedUri);

            var message = await _listController.ExecuteAction(controller => controller.PostAsync(newItem));
            message.TryGetContentValue(out IListItem item);

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
            var newItem = new ListItemViewModel { Text = string.Empty};

            var message = await _listController.ExecuteAction(controller => controller.PostAsync(newItem));
            message.TryGetContentValue(out HttpError error);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(error.ModelState.Keys.Contains(nameof(IListItem.Text)), Is.True);
            });
        }

        [Test]
        public async Task Post_ItemWithNonemptyTextAndId_BadRequestReturned()
        {
            var newItem = new ListItemViewModel { Id = new Guid("22AC59B7-9517-4EDD-9DDD-EB418A7C1689"), Text = "aaa" };

            var message = await _listController.ExecuteAction(controller => controller.PostAsync(newItem));
            message.TryGetContentValue(out HttpError error);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(error.ModelState.Keys.Contains(nameof(IListItem.Id)), Is.True);
            });
        }


        [Test]
        public async Task Post_ItemWithNonemptyTextAndCreationTime_BadRequestReturned()
        {
            var newItem = new ListItemViewModel { Text = "aaa", CreationTime = new DateTime(2014,12,31)};

            var message = await _listController.ExecuteAction(controller => controller.PostAsync(newItem));
            message.TryGetContentValue(out HttpError error);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(error.ModelState.Keys.Contains(nameof(IListItem.CreationTime)), Is.True);
            });
        }

        [Test]
        public async Task Post_ItemWithNonemptyTextAndCreationTimeAndId_BadRequestReturned()
        {
            var newItem = new ListItemViewModel { Id = new Guid("22AC59B7-9517-4EDD-9DDD-EB418A7C1689"), Text = "aaa", CreationTime = new DateTime(2014, 12, 31) };

            var message = await _listController.ExecuteAction(controller => controller.PostAsync(newItem));
            message.TryGetContentValue(out HttpError error);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(error.ModelState.Keys.Contains(nameof(IListItem.Id)), Is.True);
                Assert.That(error.ModelState.Keys.Contains(nameof(IListItem.CreationTime)), Is.True);
            });
        }

        [Test]
        public async Task Post_ItemWithNonemptyTextAndCreationTimeAnLastUpdateTime_BadRequestReturned()
        {
            var newItem = new ListItemViewModel { Text = "aaa", CreationTime = new DateTime(2014, 12, 31), LastUpdateTime = new DateTime(2018, 11, 25) };

            var message = await _listController.ExecuteAction(controller => controller.PostAsync(newItem));
            message.TryGetContentValue(out HttpError error);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(error.ModelState.Keys.Contains(nameof(IListItem.LastUpdateTime)), Is.True);
                Assert.That(error.ModelState.Keys.Contains(nameof(IListItem.CreationTime)), Is.True);
            });
        }

        [Test]
        public async Task Post_ItemWithEmptyText_BadRequestReturned()
        {
            var newItem = new ListItemViewModel { Text = String.Empty };

            var message = await _listController.ExecuteAction(controller => controller.PostAsync(newItem));
            message.TryGetContentValue(out HttpError error);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(error.ModelState.Keys.Contains(nameof(IListItem.Text)), Is.True);
            });
        }

        [Test]
        public async Task Post_ItemWithEmptyTextAndId_BadRequestReturned()
        {
            var newItem = new ListItemViewModel { Id = new Guid("22AC59B7-9517-4EDD-9DDD-EB418A7C1689"), Text = String.Empty };

            var message = await _listController.ExecuteAction(controller => controller.PostAsync(newItem));
            message.TryGetContentValue(out HttpError error);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(error.ModelState.Keys.Contains(nameof(IListItem.Text)), Is.True);
                Assert.That(error.ModelState.Keys.Contains(nameof(IListItem.Id)), Is.True);
            });
        }

        [Test]
        public async Task Put_NoContentReturned()
        {
            var id = new Guid("22AC59B7-9517-4EDD-9DDD-EB418A7C1689");
            var item = new ListItemViewModel { Text = "newItem" };
            _cache.ExistsItemWithIdAsync(id).Returns(true);

            var message = await _listController.ExecuteAction(controller => controller.PutAsync(id, item));

            Assert.Multiple(async () =>
            {
                await _editingService.Received().ReplaceItemAsync(id, item);
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
            });
        }

        [Test]
        public async Task Put_BadRequestReturned()
        {
            var id = Guid.Empty;

            var message = await _listController.ExecuteAction(controller => controller.PutAsync(id, null));

            Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task Put_NonexistingIdItem_CreatedReturned()
        {
            var id = new Guid("22AC59B7-9517-4EDD-9DDD-EB418A7C1689");
            var item = new ListItemViewModel { Text = "newItem" };
            var createdItem = _items[0];
            _cache.ExistsItemWithIdAsync(id).Returns(false);
            var expectedUri = new Uri($"http://www.aaa.com/{createdItem.Id}");
            _additionService.AddItemAsync(item).Returns(createdItem);
            _location.GetListItemLocation(createdItem.Id).Returns(expectedUri);

            var message = await _listController.ExecuteAction(controller => controller.PutAsync(id, item));
            message.TryGetContentValue(out IListItem returnedItem);

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
                Assert.That(error.ModelState.Keys.Contains(nameof(IListItem)), Is.True);
            });
        }

        [Test]
        public async Task Put_ItemOnlyWithEmptyText_BadRequestReturned()
        {
            var item = new ListItemViewModel { Text = string.Empty };

            var message = await _listController.ExecuteAction(controller => controller.PutAsync(_items[0].Id, item));
            message.TryGetContentValue(out HttpError error);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(error.ModelState.Keys.Contains(nameof(IListItem.Text)), Is.True);
            });
        }

        [Test]
        public async Task Put_ItemWithNonemptyTextAndId_BadRequestReturned()
        {
            var item = new ListItemViewModel { Id = new Guid("22AC59B7-9517-4EDD-9DDD-EB418A7C1689"), Text = "aaa" };

            var message = await _listController.ExecuteAction(controller => controller.PutAsync(item.Id, item));
            message.TryGetContentValue(out HttpError error);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(error.ModelState.Keys.Contains(nameof(IListItem.Id)), Is.True);
            });
        }


        [Test]
        public async Task Put_ItemWithNonemptyTextAndCreationTime_BadRequestReturned()
        {
            var item = new ListItemViewModel { Text = "aaa", CreationTime = new DateTime(2014, 12, 31) };

            var message = await _listController.ExecuteAction(controller => controller.PutAsync(_items[0].Id, item));
            message.TryGetContentValue(out HttpError error);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(error.ModelState.Keys.Contains(nameof(IListItem.CreationTime)), Is.True);
            });
        }

        [Test]
        public async Task Put_ItemWithNonemptyTextAndCreationTimeAndId_BadRequestReturned()
        {
            var item = new ListItemViewModel { Id = new Guid("22AC59B7-9517-4EDD-9DDD-EB418A7C1689"), Text = "aaa", CreationTime = new DateTime(2014, 12, 31) };

            var message = await _listController.ExecuteAction(controller => controller.PutAsync(_items[0].Id, item));
            message.TryGetContentValue(out HttpError error);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(error.ModelState.Keys.Contains(nameof(IListItem.Id)), Is.True);
                Assert.That(error.ModelState.Keys.Contains(nameof(IListItem.CreationTime)), Is.True);
            });
        }

        [Test]
        public async Task Put_ItemWithNonemptyTextAndCreationTimeAnLastUpdateTime_BadRequestReturned()
        {
            var item = new ListItemViewModel { Text = "aaa", CreationTime = new DateTime(2014, 12, 31), LastUpdateTime = new DateTime(2018, 11, 25) };

            var message = await _listController.ExecuteAction(controller => controller.PutAsync(_items[0].Id, item));
            message.TryGetContentValue(out HttpError error);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(error.ModelState.Keys.Contains(nameof(IListItem.LastUpdateTime)), Is.True);
                Assert.That(error.ModelState.Keys.Contains(nameof(IListItem.CreationTime)), Is.True);
            });
        }

        [Test]
        public async Task Put_ItemWithEmptyTextAndId_BadRequestReturned()
        {
            var item = new ListItemViewModel { Id = _items[0].Id, Text = String.Empty };

            var message = await _listController.ExecuteAction(controller => controller.PutAsync(item.Id, item));
            message.TryGetContentValue(out HttpError error);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(error.ModelState.Keys.Contains(nameof(IListItem.Text)), Is.True);
                Assert.That(error.ModelState.Keys.Contains(nameof(IListItem.Id)), Is.True);
            });
        }

        [Test]
        public async Task Delete_NoContentReturned()
        {
            var id = new Guid("22AC59B7-9517-4EDD-9DDD-EB418A7C1678");
            _cache.ExistsItemWithIdAsync(id).Returns(true);

            var message = await _listController.ExecuteAction(controller => controller.DeleteAsync(id));

            await _cache.Received().DeleteItemAsync(id);
            Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test]
        public async Task Delete_BadRequestReturned()
        {
            var id = Guid.Empty;

            var message = await _listController.ExecuteAction(controller => controller.DeleteAsync(id));
            message.TryGetContentValue(out HttpError error);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(error.ModelState.Keys.Contains(nameof(IListItem.Id)), Is.True);
            });
        }

        [Test]
        public async Task Delete_NotFoundReturned()
        {
            var id = new Guid("22AC59B7-9517-4EDD-9DDD-EB418A7C1678");
            _cache.ExistsItemWithIdAsync(id).Returns(false);

            var message = await _listController.ExecuteAction(controller => controller.DeleteAsync(id));

            Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }
    }
}