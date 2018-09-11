using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Web.Http;
using MyPerfectOnboarding.Contracts;
using MyPerfectOnboarding.Contracts.Models;

namespace MyPerfectOnboarding.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/List")]
    public class ListController : ApiController
    {
        private IListRepository _listRepository;

        public ListController(IListRepository listRepository)
        {
            _listRepository = listRepository;
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetAsync() => await Task.FromResult(Ok(_listRepository.GetAllItemsAsync()));

        [HttpGet]
        public async Task<IHttpActionResult> GetAsync(Guid id) => await Task.FromResult(Ok(_listRepository.GetItemAsync(id)));

        [HttpPost]
        public async Task<IHttpActionResult> PostAsync(ListItem item) => await Task.FromResult(Created("api/v{version}/List", item));

        [HttpPut]
        public async Task<IHttpActionResult> PutAsync(Guid id, ListItem editedItem)
        {
            await _listRepository.EditItemAsync(id, editedItem);
            return await Task.FromResult(StatusCode(HttpStatusCode.NoContent));
        }

        [HttpDelete]
        public async Task<IHttpActionResult> DeleteAsync(Guid id)
        {
            await _listRepository.DeleteItemAsync(id);
            return await Task.FromResult(StatusCode(HttpStatusCode.NoContent));
        }
    }
}
