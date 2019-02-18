using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Web.Http;
using MyPerfectOnboarding.Contracts.Models;
using MyPerfectOnboarding.Contracts.Services.ListItem;
using MyPerfectOnboarding.Contracts.Services.Location;

namespace MyPerfectOnboarding.Api.Controllers
{
    [ApiVersion("1.0")]
    [RoutePrefix("api/v{version:apiVersion}/List")]
    [Route("")]
    public class ListController : ApiController
    {
        
        private readonly IUrlLocator _urlLocation;
        private readonly IAdditionService _additionService;
        private readonly IListCache _cache;

        public ListController(IUrlLocator urlLocation, IAdditionService additionService, IListCache cache)
        {
            _urlLocation = urlLocation;
            _additionService = additionService;
            _cache = cache;
        }

        public async Task<IHttpActionResult> GetAsync()
            => Ok(await _cache.GetAllItemsAsync());

        [Route("{id}", Name = "GetListItem")]
        public async Task<IHttpActionResult> GetAsync(Guid id)
        {
            var item = await _cache.GetItemAsync(id);
            if (item == null)
                return NotFound();

            return Ok(item);
        }

        public async Task<IHttpActionResult> PostAsync(ListItem item)
        {
            var newItem = await _additionService.AddItemAsync(item);
            var location = _urlLocation.GetListItemLocation(newItem.Id);

            return Created(location, newItem);
        }

        [Route("{id}")]
        public async Task<IHttpActionResult> PutAsync(Guid id, ListItem editedItem)
        {
            await _cache.ReplaceItemAsync(editedItem);

            return StatusCode(HttpStatusCode.NoContent);
        }

        [Route("{id}")]
        public async Task<IHttpActionResult> DeleteAsync(Guid id)
        {
            await _cache.DeleteItemAsync(id);

            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
