using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace MyPerfectOnboarding.Api.Tests.Utils
{
    public static class ApiControllerExtension
    {
        public static async Task<HttpResponseMessage> ExecuteAction(this ApiController controller, Func<ApiController, Task<IHttpActionResult>> action)
        {
            var response = await action(controller);

            return await response.ExecuteAsync(CancellationToken.None);
        }
    }
}
