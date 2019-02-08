using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Web.Http;
using MyPerfectOnboarding.Api.Extensions;
using MyPerfectOnboarding.Api.Models;
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
            ValidateId(id, "Id is invalid. It should not be empty.", shouldBeEmpty:false);
            if (!ModelState.IsValid)
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
            if (item == null)
            {
                ModelState.AddModelError(nameof(IListItem), "Item should not be null.");
                return BadRequest(ModelState);
            }

            ValidateBeforeAddition(item);

            if (!ModelState.IsValid)
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
            if (editedItem == null)
            {
                ModelState.AddModelError(nameof(IListItem), "Item should not be null.");
                return BadRequest(ModelState);
            }

            ValidateBeforeEditing(id, editedItem);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await _cache.ExistsItemWithIdAsync(id))
            {
                return await PostAsync(editedItem);
            }

            await _editingService.ReplaceItemAsync(id, editedItem.AsImmutable());

            return StatusCode(HttpStatusCode.NoContent);
        }

        [Route("{id}")]
        public async Task<IHttpActionResult> DeleteAsync(Guid id)
        {
            ValidateId(id, "Id is invalid. It should not be empty.", shouldBeEmpty: false);
            if (!ModelState.IsValid)
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

        private static bool IsIdEmptyGuid(Guid id) 
            => id == Guid.Empty;

        private void ValidateTime(DateTime time, string name, string errorMessage)
        {
            if (time != DateTime.MinValue)
            {
                ModelState.AddModelError(name, errorMessage);
            }
        }

        private void ValidateId(Guid id, string errorMessage, bool shouldBeEmpty)
        {
            if ((IsIdEmptyGuid(id) && !shouldBeEmpty) || (!IsIdEmptyGuid(id) && shouldBeEmpty))
            {
                ModelState.AddModelError(nameof(IListItem.Id), errorMessage);
            }
        }

        private void ValidateText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                ModelState.AddModelError(nameof(IListItem.Text), "Text was empty.");
            }
        }

        private void ValidateBeforeEditing(Guid id, ListItemViewModel item)
        {
            ValidateText(item.Text);

            ValidateTime(item.CreationTime, nameof(IListItem.CreationTime),
                "Creation time should be DateTime.MinValue.");

            ValidateTime(item.LastUpdateTime, nameof(IListItem.LastUpdateTime),
                "Last update time should be DateTime.MinValue.");

            ValidateId(item.Id, "Item id is invalid. It should be empty.", shouldBeEmpty: true);

            ValidateId(id, "Id of request is invalid. It should not be empty.", shouldBeEmpty: false);

        }

        private void ValidateBeforeAddition(ListItemViewModel item)
        {
            ValidateText(item.Text);

            ValidateTime(item.CreationTime, nameof(IListItem.CreationTime),
                "Creation time should be DateTime.MinValue.");

            ValidateTime(item.LastUpdateTime, nameof(IListItem.LastUpdateTime),
                "Last update time should be DateTime.MinValue.");

            ValidateId(item.Id, "Item id is invalid. It should be empty.", shouldBeEmpty: true);
        }
    }
}
