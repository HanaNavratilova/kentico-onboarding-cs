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

        public async Task<IHttpActionResult> GetAsync()
            => Ok(await _listRepository.GetAllItemsAsync());

        public async Task<IHttpActionResult> GetAsync(Guid id)
            => Ok(await _listRepository.GetItemAsync(id));

        public async Task<IHttpActionResult> PostAsync(ListItem item)
        {
            var newItem = await _listRepository.AddItemAsync(item);
            const string location = "api/v{version}/List/id";
            //return CreatedAtRoute("name", new { id = newItem.Id }, newItem);
            return Created(location, newItem);
            //return Created(Request.RequestUri.AbsoluteUri + newItem.Id, newItem);
        }

        public async Task<IHttpActionResult> PutAsync(Guid id, ListItem editedItem)
        {
            await _listRepository.EditItemAsync(id, editedItem);
            return StatusCode(HttpStatusCode.NoContent);
        }

        public async Task<IHttpActionResult> DeleteAsync(Guid id)
        {
            await _listRepository.DeleteItemAsync(id);
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
