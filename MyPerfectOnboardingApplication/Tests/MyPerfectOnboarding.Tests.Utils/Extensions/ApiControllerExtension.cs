using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace MyPerfectOnboarding.Tests.Utils.Extensions
{
    public static class ApiControllerExtension
    {
        public static async Task<HttpResponseMessage> ExecuteAction<TController>(this TController controller, Func<TController, Task<IHttpActionResult>> action)
            where TController: ApiController
        {
            var response = await action(controller);

            return await response.ExecuteAsync(CancellationToken.None);
        }
    }
}
