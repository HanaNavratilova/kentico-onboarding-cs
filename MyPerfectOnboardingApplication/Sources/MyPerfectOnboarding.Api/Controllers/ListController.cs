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
        private readonly IPostService _postService;
        private readonly IPutService _putService;
        private readonly IListCache _cache;

        public ListController(IUrlLocator urlLocation, IPostService postService, IPutService putService, IListCache cache)
        {
            _urlLocation = urlLocation;
            _postService = postService;
            _putService = putService;
            _cache = cache;
        }

        public async Task<IHttpActionResult> GetAsync()
            => Ok(await _cache.GetAllItemsAsync());

        [Route("{id}", Name = "GetListItem")]
        public async Task<IHttpActionResult> GetAsync(Guid id)
        {
            ValidateId(id, "Id is invalid.", shouldBeEmpty:false);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var item = await _cache.GetItemAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        public async Task<IHttpActionResult> PostAsync(ListItem item)
        {
            if (item == null)
            {
                ModelState.AddModelError(nameof(ListItem), "Should not be null.");
                return BadRequest(ModelState);
            }

            ValidateBeforeAddition(item);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newItem = await _postService.AddItemAsync(item);
            var location = _urlLocation.GetListItemLocation(newItem.Id);

            return Created(location, newItem);
        }

        [Route("{id}")]
        public async Task<IHttpActionResult> PutAsync(Guid id, ListItem editedItem)
        {
            if (editedItem == null)
            {
                ModelState.AddModelError(nameof(ListItem), "Should not be null.");
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

            await _putService.ReplaceItemAsync(editedItem);

            return StatusCode(HttpStatusCode.NoContent);
        }

        [Route("{id}")]
        public async Task<IHttpActionResult> DeleteAsync(Guid id)
        {
            ValidateId(id, "Id is invalid.", shouldBeEmpty: false);
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
                ModelState.AddModelError(nameof(ListItem.Id), errorMessage);
            }
        }

        private void ValidateText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                ModelState.AddModelError(nameof(ListItem.Text), "Text was empty.");
            }
        }

        private void ValidateBeforeEditing(Guid id, ListItem item)
        {
            ValidateText(item.Text);

            ValidateTime(item.CreationTime, nameof(ListItem.CreationTime),
                "Creation time should be DateTime.MinValue.");

            ValidateTime(item.LastUpdateTime, nameof(ListItem.LastUpdateTime),
                "Last update time should be DateTime.MinValue.");

            ValidateId(item.Id, "Id is invalid.", shouldBeEmpty: true);

            ValidateId(id, "Id is invalid.", shouldBeEmpty: false);

        }

        private void ValidateBeforeAddition(ListItem item)
        {
            ValidateText(item.Text);

            ValidateTime(item.CreationTime, nameof(ListItem.CreationTime),
                "Creation time should be DateTime.MinValue.");

            ValidateTime(item.LastUpdateTime, nameof(ListItem.LastUpdateTime),
                "Last update time should be DateTime.MinValue.");

            ValidateId(item.Id, "Id should be Guid.Empty.", shouldBeEmpty: true);
        }
    }
}
