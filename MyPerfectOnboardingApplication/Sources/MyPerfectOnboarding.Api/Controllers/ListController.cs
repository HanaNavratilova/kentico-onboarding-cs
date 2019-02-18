using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Web.Http;
using MyPerfectOnboarding.Contracts.Models;
using MyPerfectOnboarding.Contracts.Services.ListItems;
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
        private readonly IEditingService _editingService;
        private readonly IListCache _cache;

        public ListController(IUrlLocator urlLocation, IAdditionService additionService, IEditingService editingService, IListCache cache)
        {
            _urlLocation = urlLocation;
            _additionService = additionService;
            _editingService = editingService;
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
            var item = await _editingService.ReplaceItemAsync(id, editedItem);

            return Ok(item);
        }

        [Route("{id}")]
        public async Task<IHttpActionResult> DeleteAsync(Guid id)
        {
            await _cache.DeleteItemAsync(id);

            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
