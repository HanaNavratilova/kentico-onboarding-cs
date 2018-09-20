using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Web.Http;
using MyPerfectOnboarding.Contracts.Database;
using MyPerfectOnboarding.Contracts.Models;
using MyPerfectOnboarding.Contracts.Services.Location;

namespace MyPerfectOnboarding.Api.Controllers
{
    [ApiVersion("1.0")]
    [RoutePrefix("api/v{version:apiVersion}/List")]
    [Route("")]
    public class ListController : ApiController
    {
        private readonly IListRepository _listRepository;
        private readonly IUrlLocator _urlLocation;

        public ListController(IListRepository listRepository, IUrlLocator urlLocation)
        {
            _listRepository = listRepository;
            _urlLocation = urlLocation;
        }

        public async Task<IHttpActionResult> GetAsync()
            => Ok(await _listRepository.GetAllItemsAsync());

        [Route("{id}", Name = "GetListItem")]
        public async Task<IHttpActionResult> GetAsync(Guid id) 
            => Ok(await _listRepository.GetItemAsync(id));

        public async Task<IHttpActionResult> PostAsync(ListItem item)
        {
            var newItem = await _listRepository.AddItemAsync(item);
            var location = _urlLocation.GetListItemLocation(newItem.Id);

            return Created(location, newItem);
        }

        [Route("{id}")]
        public async Task<IHttpActionResult> PutAsync(Guid id, ListItem editedItem)
        {
            await _listRepository.EditItemAsync(id, editedItem);

            return StatusCode(HttpStatusCode.NoContent);
        }

        [Route("{id}")]
        public async Task<IHttpActionResult> DeleteAsync(Guid id)
        {
            await _listRepository.DeleteItemAsync(id);

            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
