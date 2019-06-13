using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Web.Http;
using MyPerfectOnboarding.Api.Extensions;
using MyPerfectOnboarding.Api.Models;
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
            => Ok(await _cache.GetAllItemsAsync().ToViewModelsAsync());

        [Route("{id}", Name = "GetListItem")]
        public async Task<IHttpActionResult> GetAsync(Guid id)
        {
            if (!ModelState.ValidateRequestId(id).IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await _cache.ExistsItemWithIdAsync(id))
            {
                return NotFound();
            }

            var item = await _cache.GetItemAsync(id);

            return Ok(item.ToViewModel());
        }

        public async Task<IHttpActionResult> PostAsync(ListItemViewModel item)
        {
            if (!ModelState.ValidateBeforeAddition(item).IsValid)
            {
                return BadRequest(ModelState);
            }

            var newItem = await _additionService.AddItemAsync(item.AsImmutable());
            var location = _urlLocation.GetListItemLocation(newItem.Id);

            return Created(location, newItem.ToViewModel());
        }

        [Route("{id}")]
        public async Task<IHttpActionResult> PutAsync(Guid id, ListItemViewModel editedItem)
        {
            if (!ModelState.ValidateBeforeEditing(id, editedItem).IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await _cache.ExistsItemWithIdAsync(id))
            {
                return await PostAsync(editedItem);
            }

            var item = await _editingService.ReplaceItemAsync(id, editedItem.AsImmutable());

            return Ok(item.ToViewModel());
        }

        [Route("{id}")]
        public async Task<IHttpActionResult> DeleteAsync(Guid id)
        {
            if (!ModelState.ValidateRequestId(id).IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await _cache.ExistsItemWithIdAsync(id))
            {
                return NotFound();
            }

            await _cache.DeleteItemAsync(id);

            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
