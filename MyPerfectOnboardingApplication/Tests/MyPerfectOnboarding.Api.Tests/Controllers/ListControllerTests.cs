using MyPerfectOnboarding.Api.Controllers;
using MyPerfectOnboarding.Contracts.Models;
using MyPerfectOnboarding.Contracts.Services.Location;
using MyPerfectOnboarding.Tests.Utils.Extensions;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using MyPerfectOnboarding.Api.Models;
using MyPerfectOnboarding.Api.Tests.Controllers.TestCasesData;
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
            var expectedItem = ListItemBuilder.CreateItem("E1AC8C1A-5E51-4B63-B982-37D1D04705A1", "aaaaa", "1589-12-03", "1896-04-07", true);
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
            var id = new Guid("DE181743-EE7C-408B-BC3B-9217622625B1");
            _cache.ExistsItemWithIdAsync(id).Returns(false);

            var message = await _listController.ExecuteAction(controller => controller.GetAsync(id));

            Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task Post_ItemOnlyWithNonemptyText_CreatedReturned()
        {
            var createdItem = ListItemBuilder.CreateItem("EE7DAC75-B63B-4FD9-9EB4-9F3641438290", "aaaaa", "1589-12-03", "1896-04-07", true);
            var newItem = ViewModelItem.WithText;
            var expectedUri = new Uri($"http://www.aaa.com/{createdItem.Id}");
            _additionService.AddItemAsync(ArgExtended.IsListItem(newItem)).Returns(createdItem);
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

        [TestCaseSource(typeof(PostBadRequestTestCases))]
        public async Task Post_BadRequestReturned(ListItemViewModel item, string[] expectedModelStateKeys)
        {
            var message = await _listController.ExecuteAction(controller => controller.PostAsync(item));
            message.TryGetContentValue(out HttpError error);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(error.ModelState.Keys, Is.EquivalentTo(expectedModelStateKeys));
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
        public async Task Put_NonexistingIdItem_CreatedReturned()
        {
            var id = new Guid("9DD010C6-8044-438C-B293-643102935565");
            var item = ViewModelItem.WithText;
            var createdItem = ListItemBuilder.CreateItem("62F9BB31-AE2F-4E73-AA70-B373F72BE754", "aaaaa", "1589-12-03", "1896-04-07", true);
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

        [TestCaseSource(typeof(PutBadRequestTestCases))]
        public async Task Put_BadRequestReturned(Guid id, ListItemViewModel item, string[] expectedModelStateKeys)
        {
            var message = await _listController.ExecuteAction(controller => controller.PutAsync(id, item));
            message.TryGetContentValue(out HttpError error);

            Assert.Multiple(() =>
            {
                Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(error.ModelState.Keys, Is.EquivalentTo(expectedModelStateKeys));
            });
        }

        [Test]
        public async Task Delete_Id_NoContentReturned()
        {
            var id = new Guid("54109CC2-D8A1-497E-BD65-F76DE46D86E4");
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
            var id = new Guid("A0BF4FCF-6610-4103-8423-74EA5A0F56A3");
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