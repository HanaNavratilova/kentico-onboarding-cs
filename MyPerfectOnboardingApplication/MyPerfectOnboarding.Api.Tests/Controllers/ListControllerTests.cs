using System;
using System.Collections.Generic;
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
            var message = await _listController.ExecuteAction(controller => controller.GetAsync());

            List <ListItem> items;
            message.TryGetContentValue(out items);

            Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Get_ReturnsItemWithGivenId()
        {
            var id = new Guid();

            var message = await _listController.ExecuteAction(controller => controller.GetAsync(id));

            Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Post_CreatedReturned()
        {
            var newItem = new ListItem{Id = Guid.NewGuid(), Text = "newItem", IsActive = false, CreationTime = DateTime.Now, LastUpdateTime = DateTime.Now};

            var message = await _listController.ExecuteAction(controller => controller.PostAsync(newItem));

            Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        }

        [Test]
        public async Task Put_OkReturned()
        {
            var id = new Guid();

            var item = new ListItem{ Id = id, Text = "newItem", IsActive = false, CreationTime = DateTime.Now, LastUpdateTime = DateTime.Now };

            var message = await _listController.ExecuteAction(controller => controller.PutAsync(id, item));

            Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test]
        public async Task Delete_NoContentReturned()
        {
            var id = new Guid();

            var message = await _listController.ExecuteAction(controller => controller.DeleteAsync(id));

            Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }
    }
}
