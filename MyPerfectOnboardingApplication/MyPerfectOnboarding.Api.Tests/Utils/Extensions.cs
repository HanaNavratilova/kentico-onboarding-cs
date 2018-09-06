using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace MyPerfectOnboarding.Api.Tests.Utils
{
    public static class Extensions
    {
        public static async Task<HttpResponseMessage> ExecuteAction(this IHttpActionResult response)
        {
            return await response.ExecuteAsync(CancellationToken.None);
        }
    }
}
