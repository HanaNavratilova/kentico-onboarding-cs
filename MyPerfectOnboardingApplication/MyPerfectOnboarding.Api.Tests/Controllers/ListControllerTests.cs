using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using MyPerfectOnboarding.Api.Controllers;
using MyPerfectOnboarding.Api.Models;
using MyPerfectOnboarding.Api.Tests.Utils;
using NUnit.Framework;

namespace MyPerfectOnboarding.Api.Tests.Controllers
{
    [TestFixture]
    class ListControllerTests
    {
        private ListController _listController;

        [SetUp]
        public void Init()
        {
            _listController = new ListController
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };
        }

        [Test]
        public async Task Get_ReturnsListOfItems()
        {
            var response = await _listController.GetAsync();

            var message = await response.ExecuteAction();

            List<ListItem> items;

            message.TryGetContentValue(out items);

            Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            Assert.That(items, Is.InstanceOf<List<ListItem>>());
        }

        [Test]
        public async Task Get_ReturnsItemWithGivenId()
        {
            Guid id = _listController.items.First().Id;

            var response = await _listController.GetAsync(id);

            var message = await response.ExecuteAction();

            ListItem item;

            message.TryGetContentValue(out item);

            Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            Assert.That(item, Is.InstanceOf<ListItem>());
            Assert.That(item.Id, Is.EqualTo(id));
        }

        [Test]
        public async Task Post_CreatedReturned()
        {
            var newItem = new ListItem(Guid.NewGuid(), "newItem", false, DateTime.Now, DateTime.Now);

            var response = await _listController.PostAsync(newItem);

            var message = await response.ExecuteAction();

            Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        }

        [Test]
        public async Task Put_OkReturned()
        {
            var index = 1;

            var id = _listController.items.ElementAt(index).Id;

            var item = new ListItem(id, "item", false, DateTime.Now, DateTime.Now);

            var response = await _listController.PutAsync(id, item);

            var message = await response.ExecuteAction();

            Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Delete_NoContentReturned()
        {
            var index = 1;

            var id = _listController.items.ElementAt(index).Id;

            var response = await _listController.DeleteAsync(id);

            var message = await response.ExecuteAction();

            Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }
    }
}
