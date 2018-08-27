using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Build.Tasks;
using MyPerfectOnboarding.Api.Controllers;
using MyPerfectOnboarding.Api.Models;
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

            var message = await response.ExecuteAsync(CancellationToken.None);

            List<ListItem> items;

            message.TryGetContentValue<List<ListItem>>(out items);

            Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            Assert.That(items, Is.InstanceOf<List<ListItem>>());
        }

        [Test]
        public async Task Get_ReturnsItemWithGivenId()
        {
            Guid id = _listController.items.First().Id;

            var response = await _listController.GetAsync(id);

            var message = await response.ExecuteAsync(CancellationToken.None);

            ListItem item;

            message.TryGetContentValue<ListItem>(out item);

            Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            Assert.That(item, Is.InstanceOf<ListItem>());
            Assert.That(item.Id, Is.EqualTo(id));
        }

        [Test]
        public async Task Post_OkReturned()
        {
            var newItem = new ListItem(Guid.NewGuid(), "newItem", false, DateTime.Now, DateTime.Now);

            var response = await _listController.PostAsync(newItem);

            var message = await response.ExecuteAsync(CancellationToken.None);

            Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Put_OkReturned()
        {
            var index = 1;

            var id = _listController.items.ElementAt(index).Id;

            var item = new ListItem(id, "item", false, DateTime.Now, DateTime.Now);

            var response = await _listController.PutAsync(id, item);

            var message = await response.ExecuteAsync(CancellationToken.None);

            Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Delete_OkReturned()
        {
            var index = 1;

            var id = _listController.items.ElementAt(index).Id;

            var response = await _listController.DeleteAsync(id);

            var message = await response.ExecuteAsync(CancellationToken.None);

            Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }
    }
}
