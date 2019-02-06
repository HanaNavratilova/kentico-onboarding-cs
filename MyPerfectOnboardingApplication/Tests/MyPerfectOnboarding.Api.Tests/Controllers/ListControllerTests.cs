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
using MyPerfectOnboarding.Tests.Utils.Builders;
using MyPerfectOnboarding.Tests.Utils.Comparers;

namespace MyPerfectOnboarding.Api.Tests.Controllers
{
    [TestFixture]
    internal class ListControllerTests
    {
        private ListController _listController;
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
            ListItem[] items =
            {
                ListItemBuilder.CreateItem("0B9E6EAF-83DC-4A99-9D57-A39FAF258CAC", "aaaaa", "1589-12-03", "1896-04-07"),
                ListItemBuilder.CreateItem("11AC59B7-9517-4EDD-9DDD-EB418A7C1644", "dfads", "4568-06-23", "8569-08-24")
            };

            _cache.GetAllItemsAsync().Returns(items);

            var message = await _listController.ExecuteAction(controller => controller.GetAsync());
            message.TryGetContentValue(out IEnumerable<IListItem> resultItems);

            Assert.Multiple(() => { 
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(resultItems, Is.EqualTo(items).UsingListItemComparer());
            });
        }

        [Test]
        public async Task Get_ExistingId_ReturnsItemWithGivenId()
        {
            var expectedItem = ListItemBuilder.CreateItem("0B9E6EAF-83DC-4A99-9D57-A39FAF258CAC", "aaaaa", "1589-12-03", "1896-04-07", true);
            _cache.ExistsItemWithIdAsync(expectedItem.Id).Returns(true);
            _cache.GetItemAsync(expectedItem.Id).Returns(Task.FromResult(expectedItem));

            var message = await _listController.ExecuteAction(controller => controller.GetAsync(expectedItem.Id));
            message.TryGetContentValue(out IListItem item);

            Assert.Multiple(() => {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(item, Is.EqualTo(expectedItem).UsingListItemComparer());
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
                Assert.That(error.ModelState, Does.ContainKey(nameof(IListItem.Id)));
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
            var createdItem = ListItemBuilder.CreateItem("0B9E6EAF-83DC-4A99-9D57-A39FAF258CAC", "aaaaa", "1589-12-03", "1896-04-07", true);
            var newItem = ListItemViewModelBuilder.CreateItem("newItemText");
            var expectedUri = new Uri($"http://www.aaa.com/{createdItem.Id}");
            _additionService.AddItemAsync(Arg.Is<ListItem>(listItem => ListItemEqualityComparer.Instance.Equals(listItem, newItem))).Returns(createdItem);
            _location.GetListItemLocation(createdItem.Id).Returns(expectedUri);

            var message = await _listController.ExecuteAction(controller => controller.PostAsync(newItem));
            message.TryGetContentValue(out IListItem item);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.Created));
                Assert.That(message.Headers.Location, Is.EqualTo(expectedUri));
                Assert.That(item, Is.EqualTo(createdItem).UsingListItemComparer());
            });
        }

        [Test]
        public async Task Post_ItemOnlyWithEmptyText_BadRequestReturned()
        {
            var newItem = ListItemViewModelBuilder.CreateItem(string.Empty);

            var message = await _listController.ExecuteAction(controller => controller.PostAsync(newItem));
            message.TryGetContentValue(out HttpError error);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(error.ModelState, Does.ContainKey(nameof(IListItem.Text)));
            });
        }

        [Test]
        public async Task Post_ItemWithNonemptyTextAndId_BadRequestReturned()
        {
            var newItem = ListItemViewModelBuilder.CreateItem("22AC59B7-9517-4EDD-9DDD-EB418A7C1689", "aaa");

            var message = await _listController.ExecuteAction(controller => controller.PostAsync(newItem));
            message.TryGetContentValue(out HttpError error);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(error.ModelState, Does.ContainKey(nameof(IListItem.Id)));
            });
        }


        [Test]
        public async Task Post_ItemWithNonemptyTextAndCreationTime_BadRequestReturned()
        {
            var newItem = ListItemViewModelBuilder.CreateItem(null, "aaa", "2014-12-31");

            var message = await _listController.ExecuteAction(controller => controller.PostAsync(newItem));
            message.TryGetContentValue(out HttpError error);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(error.ModelState, Does.ContainKey(nameof(IListItem.CreationTime)));
            });
        }

        [Test]
        public async Task Post_ItemWithNonemptyTextAndCreationTimeAndId_BadRequestReturned()
        {
            var newItem = ListItemViewModelBuilder.CreateItem("22AC59B7-9517-4EDD-9DDD-EB418A7C1689", "aaa", "2014-12-31");

            var message = await _listController.ExecuteAction(controller => controller.PostAsync(newItem));
            message.TryGetContentValue(out HttpError error);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(error.ModelState, Does.ContainKey(nameof(IListItem.Id)));
                Assert.That(error.ModelState, Does.ContainKey(nameof(IListItem.CreationTime)));
            });
        }

        [Test]
        public async Task Post_ItemWithNonemptyTextAndCreationTimeAnLastUpdateTime_BadRequestReturned()
        {
            var newItem = ListItemViewModelBuilder.CreateItem(null, "aaa", "2014-12-31", "2018-11-25");

            var message = await _listController.ExecuteAction(controller => controller.PostAsync(newItem));
            message.TryGetContentValue(out HttpError error);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(error.ModelState, Does.ContainKey(nameof(IListItem.LastUpdateTime)));
                Assert.That(error.ModelState, Does.ContainKey(nameof(IListItem.CreationTime)));
            });
        }

        [Test]
        public async Task Post_ItemWithEmptyText_BadRequestReturned()
        {
            var newItem = ListItemViewModelBuilder.CreateItem(string.Empty);

            var message = await _listController.ExecuteAction(controller => controller.PostAsync(newItem));
            message.TryGetContentValue(out HttpError error);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(error.ModelState, Does.ContainKey(nameof(IListItem.Text)));
            });
        }

        [Test]
        public async Task Post_ItemWithEmptyTextAndId_BadRequestReturned()
        {
            var newItem = ListItemViewModelBuilder.CreateItem("22AC59B7-9517-4EDD-9DDD-EB418A7C1689", string.Empty);

            var message = await _listController.ExecuteAction(controller => controller.PostAsync(newItem));
            message.TryGetContentValue(out HttpError error);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(error.ModelState, Does.ContainKey(nameof(IListItem.Text)));
                Assert.That(error.ModelState, Does.ContainKey(nameof(IListItem.Id)));
            });
        }

        [Test]
        public async Task Put_Id_OkReturned()
        {
            var id = new Guid("AA1B49E6-018A-4441-B1CC-8E2FA90944CD");
            var receivedItem = ViewModelItem.WithText;
            _cache.ExistsItemWithIdAsync(id).Returns(true);
            var expectedItem = ListItemBuilder.CreateItem("41CCB9DF-F43E-4C72-9F51-1C5C96D3DA84", "item", "1589-12-03", "1896-04-07", true);
            _editingService
                .ReplaceItemAsync(id, Arg.Is<ListItem>(listItem => ListItemEqualityComparer.Instance.Equals(listItem, receivedItem)))
                .Returns(expectedItem);

            var message = await _listController.ExecuteAction(controller => controller.PutAsync(id, receivedItem));
            message.TryGetContentValue(out IListItem returnedItem);

            Assert.Multiple(async () =>
            {
                await _editingService.Received().ReplaceItemAsync(id, ArgExtended.IsListItem(receivedItem));
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(returnedItem, Is.EqualTo(expectedItem).UsingListItemComparer());
            });
        }

        [Test]
        public async Task Put_InvalidId_BadRequestReturned()
        {
            var id = Guid.Empty;
            var item = ListItemViewModelBuilder.CreateItem("newItem");

            var message = await _listController.ExecuteAction(controller => ((ListController)controller).PutAsync(id, item));
            message.TryGetContentValue(out ListItem resultItem);

            Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task Put_NonexistingIdItem_CreatedReturned()
        {
            var id = new Guid("22AC59B7-9517-4EDD-9DDD-EB418A7C1689");
            var item = ListItemViewModelBuilder.CreateItem("newItem");
            var createdItem = ListItemBuilder.CreateItem("0B9E6EAF-83DC-4A99-9D57-A39FAF258CAC", "aaaaa", "1589-12-03", "1896-04-07", true);
            _cache.ExistsItemWithIdAsync(id).Returns(false);
            var expectedUri = new Uri($"http://www.aaa.com/{createdItem.Id}");
            _additionService
                .AddItemAsync(Arg.Is<ListItem>(listItem => ListItemEqualityComparer.Instance.Equals(listItem, item)))
                .Returns(createdItem);
            _location.GetListItemLocation(createdItem.Id).Returns(expectedUri);

            var message = await _listController.ExecuteAction(controller => controller.PutAsync(id, item));
            message.TryGetContentValue(out IListItem returnedItem);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.Created));
                Assert.That(message.Headers.Location, Is.EqualTo(expectedUri));
                Assert.That(returnedItem, Is.EqualTo(createdItem).UsingListItemComparer());
            });
        }

        [Test]
        public async Task Put_NullItem_BadRequest()
        {
            var id = new Guid("0B9E6EAF-83DC-4A99-9D57-A39FAF258CAC");

            var message = await _listController.ExecuteAction(controller => controller.PutAsync(id, null));
            message.TryGetContentValue(out HttpError error);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(error.ModelState, Does.ContainKey(nameof(IListItem)));
            });
        }

        [Test]
        public async Task Put_ItemOnlyWithEmptyText_BadRequestReturned()
        {
            var id = new Guid("0B9E6EAF-83DC-4A99-9D57-A39FAF258CAC");
            var item = ListItemViewModelBuilder.CreateItem(string.Empty);

            var message = await _listController.ExecuteAction(controller => controller.PutAsync(id, item));
            message.TryGetContentValue(out HttpError error);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(error.ModelState, Does.ContainKey(nameof(IListItem.Text)));
            });
        }

        [Test]
        public async Task Put_ItemWithNonemptyTextAndId_BadRequestReturned()
        {
            var item = ListItemViewModelBuilder.CreateItem("22AC59B7-9517-4EDD-9DDD-EB418A7C1689", "aaa");

            var message = await _listController.ExecuteAction(controller => controller.PutAsync(item.Id, item));
            message.TryGetContentValue(out HttpError error);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(error.ModelState, Does.ContainKey(nameof(IListItem.Id)));
            });
        }


        [Test]
        public async Task Put_ItemWithNonemptyTextAndCreationTime_BadRequestReturned()
        {
            var id = new Guid("0B9E6EAF-83DC-4A99-9D57-A39FAF258CAC");
            var item = ListItemViewModelBuilder.CreateItem(null, "aaa", "2014-12-31");

            var message = await _listController.ExecuteAction(controller => controller.PutAsync(id, item));
            message.TryGetContentValue(out HttpError error);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(error.ModelState, Does.ContainKey(nameof(IListItem.CreationTime)));
            });
        }

        [Test]
        public async Task Put_ItemWithNonemptyTextAndCreationTimeAndId_BadRequestReturned()
        {
            var id = new Guid("0B9E6EAF-83DC-4A99-9D57-A39FAF258CAC");
            var item = ListItemViewModelBuilder.CreateItem("22AC59B7-9517-4EDD-9DDD-EB418A7C1689", "aaa", "2014-12-31");

            var message = await _listController.ExecuteAction(controller => controller.PutAsync(id, item));
            message.TryGetContentValue(out HttpError error);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(error.ModelState, Does.ContainKey(nameof(IListItem.Id)));
                Assert.That(error.ModelState, Does.ContainKey(nameof(IListItem.CreationTime)));
            });
        }

        [Test]
        public async Task Put_ItemWithNonemptyTextAndCreationTimeAnLastUpdateTime_BadRequestReturned()
        {
            var id = new Guid("0B9E6EAF-83DC-4A99-9D57-A39FAF258CAC");
            var item = ListItemViewModelBuilder.CreateItem(null, "aaa", "2014-12-31", "2018-11-25");

            var message = await _listController.ExecuteAction(controller => controller.PutAsync(id, item));
            message.TryGetContentValue(out HttpError error);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(error.ModelState, Does.ContainKey(nameof(IListItem.LastUpdateTime)));
                Assert.That(error.ModelState, Does.ContainKey(nameof(IListItem.CreationTime)));
            });
        }

        [Test]
        public async Task Put_ItemWithEmptyTextAndId_BadRequestReturned()
        {
            var item = ListItemViewModelBuilder.CreateItem("22AC59B7-9517-4EDD-9DDD-EB418A7C1689", String.Empty);

            var message = await _listController.ExecuteAction(controller => controller.PutAsync(item.Id, item));
            message.TryGetContentValue(out HttpError error);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(error.ModelState, Does.ContainKey(nameof(IListItem.Text)));
                Assert.That(error.ModelState, Does.ContainKey(nameof(IListItem.Id)));
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
                Assert.That(error.ModelState, Does.ContainKey(nameof(IListItem.Id)));
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