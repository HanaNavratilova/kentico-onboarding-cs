using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using MyPerfectOnboarding.Api.Controllers;
using MyPerfectOnboarding.Api.Tests.Utils;
using MyPerfectOnboarding.Contracts;
using MyPerfectOnboarding.Contracts.Models;
using NSubstitute;
using NUnit.Framework;

namespace MyPerfectOnboarding.Api.Tests.Controllers
{
    [TestFixture]
    class ListControllerTests
    {
        private ListController _listController;

        private readonly ListItem[] _items = {
            new ListItem{Id = Guid.NewGuid(), Text = "aaaaa", IsActive = false, CreationTime = new DateTime(1589, 12, 3), LastUpdateTime = new DateTime(1896, 4, 7)},
            new ListItem{Id = Guid.NewGuid(), Text = "dfads", IsActive = false, CreationTime = new DateTime(4568, 6, 23), LastUpdateTime = new DateTime(8569, 8, 24)},
        };

        private IListRepository _repository;

       [SetUp]
        public void Init()
        {
            _repository = Substitute.For<IListRepository>();

            _listController = new ListController(_repository)
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };
        }

        [Test]
        public async Task Get_ReturnsListOfItems()
        {
            var message = await _listController.ExecuteAction(controller => controller.GetAsync());

           message.TryGetContentValue(out List<ListItem> items);

            Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Get_ReturnsItemWithGivenId()
        {
            var id = Guid.NewGuid();

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
            var id = Guid.NewGuid();

            var item = new ListItem{ Id = id, Text = "newItem", IsActive = false, CreationTime = DateTime.Now, LastUpdateTime = DateTime.Now };

            var message = await _listController.ExecuteAction(controller => controller.PutAsync(id, item));

            Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test]
        public async Task Delete_NoContentReturned()
        {
            var id = Guid.NewGuid();

            var message = await _listController.ExecuteAction(controller => controller.DeleteAsync(id));

            Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }
    }
}
