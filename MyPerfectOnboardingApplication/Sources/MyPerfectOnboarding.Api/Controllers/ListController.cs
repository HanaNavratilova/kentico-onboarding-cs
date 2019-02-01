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
            if (!IsIdValid(id))
                return BadRequest("Id is invalid.");

            var item = await _cache.GetItemAsync(id);
            if (item == null)
                return NotFound();

            return Ok(item);
        }

        public async Task<IHttpActionResult> PostAsync(ListItem item)
        {
            if (string.IsNullOrEmpty(item.Text))
                return BadRequest("Text was empty.");

            var newItem = await _postService.AddItemAsync(item);
            var location = _urlLocation.GetListItemLocation(newItem.Id);

            return Created(location, newItem);
        }

        [Route("{id}")]
        public async Task<IHttpActionResult> PutAsync(Guid id, ListItem editedItem)
        {
            if (!IsIdValid(id))
                return BadRequest("Id is invalid.");

            if (!await _cache.ExistsItemWithIdAsync(id))
                return NotFound();

            await _putService.ReplaceItemAsync(editedItem);

            return StatusCode(HttpStatusCode.NoContent);
        }

        [Route("{id}")]
        public async Task<IHttpActionResult> DeleteAsync(Guid id)
        {
            if (!IsIdValid(id))
            {
                return BadRequest("Id is invalid.");
            }

            if (!await _cache.ExistsItemWithIdAsync(id))
                return NotFound();

            await _cache.DeleteItemAsync(id);

            return StatusCode(HttpStatusCode.NoContent);
        }

        private static bool IsIdValid(Guid id)
        {
            return id != Guid.Empty;
        }
    }
}
