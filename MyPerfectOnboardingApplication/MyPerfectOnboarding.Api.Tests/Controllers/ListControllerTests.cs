using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Build.Tasks;
using MyPerfectOnboarding.Api.Controllers;
using NUnit.Framework;

namespace MyPerfectOnboarding.Api.Tests.Controllers
{
    [TestFixture]
    class ListControllerTests
    {
        [Test]
        public async Task Get_ReturnsListOfItems()
        {
            var listController = new ListController();

            listController.Request = new HttpRequestMessage();
            listController.Configuration = new HttpConfiguration();

            var response = await listController.GetAsync();

            var message = await response.ExecuteAsync(CancellationToken.None);

            message.TryGetContentValue(out string[] items);

            Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.OK));
         
            Assert.That(items, Is.EqualTo(new[]{ "aaaa", "lll" }));
        }
    }
}
