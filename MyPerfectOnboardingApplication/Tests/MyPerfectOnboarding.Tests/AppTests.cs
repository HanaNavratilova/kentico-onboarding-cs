using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MyPerfectOnboarding.Api.Controllers;
using MyPerfectOnboarding.Contracts.Models;
using MyPerfectOnboarding.Contracts.Services.ListItem;
using MyPerfectOnboarding.Contracts.Services.Location;
using NUnit.Framework;

namespace MyPerfectOnboarding.App.Tests
{
    [TestFixture]
    internal class AppTests
    {
        private ListController _listController;

        [SetUp]
        public void Init()
        {
            var _location = Substitute.For<IUrlLocator>();
            va _additionService = new AdditionService()
            var _editingService = new 
            var _cache = Substitute.For<IListCache>();
            var 

            _listController = new ListController(_location, _additionService, _editingService, _cache)
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };
        }

        [Test]
        public async Task Get_()
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
                Assert.That(resultItems, Is.EqualTo(items));
            });
        }
    }
}
