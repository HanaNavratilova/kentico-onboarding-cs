using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using MyPerfectOnboarding.Api.Controllers;

namespace MyPerfectOnboarding.Api.Tests.Utils
{
    public static class ListControllerExtension
    {
        public static async Task<HttpResponseMessage> ExecuteAction(this ListController controller, Func<ListController, Task<IHttpActionResult>> action)
        {
            var response = await action(controller);

            return await response.ExecuteAsync(CancellationToken.None);
        }
    }
}
