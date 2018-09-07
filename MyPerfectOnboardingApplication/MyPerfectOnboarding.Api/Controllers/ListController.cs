using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Web.Http;
using MyPerfectOnboarding.Api.Models;

namespace MyPerfectOnboarding.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/List")]
    public class ListController : ApiController
    {
        private ListItem[] items = {
            new ListItem{Id = Guid.NewGuid(), Text = "aaaa", IsActive = false, CreationTime = DateTime.MinValue, LastUpdateTime = DateTime.MinValue},
            new ListItem{Id = Guid.NewGuid(), Text = "dfads", IsActive = false, CreationTime = DateTime.MinValue, LastUpdateTime = DateTime.MinValue},
        };

        [HttpGet]
        public async Task<IHttpActionResult> GetAsync() => await Task.FromResult(Ok(items));

        [HttpGet]
        public async Task<IHttpActionResult> GetAsync(Guid id) => await Task.FromResult(Ok(items[0]));

        [HttpPost]
        public async Task<IHttpActionResult> PostAsync(ListItem item) => await Task.FromResult(Created("api/v{version}/List", item));

        [HttpPut]
        public async Task<IHttpActionResult> PutAsync(Guid id, ListItem editedItem) => await Task.FromResult(StatusCode(HttpStatusCode.NoContent));

        [HttpDelete]
        public async Task<IHttpActionResult> DeleteAsync(Guid id) => await Task.FromResult(StatusCode(HttpStatusCode.NoContent));
    }
}
