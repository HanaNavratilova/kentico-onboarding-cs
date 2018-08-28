using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Caching;
using System.Web.Http;
using Microsoft.Web.Http;
using MyPerfectOnboarding.Api.Models;

namespace MyPerfectOnboarding.Api.Controllers
{
    [ApiVersion("1.0")]
    [RoutePrefix("api/v{version:apiVersion}/List")]
    public class ListController : ApiController
    {
        public List<ListItem> items = new List<ListItem>
        {
            new ListItem(Guid.NewGuid(), "aaaa", false, DateTime.MinValue, DateTime.MinValue),
            new ListItem(Guid.NewGuid(), "dfads", false, DateTime.MinValue, DateTime.MinValue)
        };

        [HttpGet]
        public async Task<IHttpActionResult> GetAsync()
        {
            return await Task.FromResult((IHttpActionResult)Ok(items));
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetAsync(Guid id)
        {
            var listItem = items.Find(item => item.Id == id);
            return await Task.FromResult((IHttpActionResult)Ok(listItem));
        }

        [HttpPost]
        public async Task<IHttpActionResult> PostAsync(ListItem item)
        {
            items.Add(item);
            return await Task.FromResult((IHttpActionResult)Ok());
        }

        [HttpPut]
        public async Task<IHttpActionResult> PutAsync(Guid id, ListItem editedItem)
        {
            var originalItem = items.Find(item => item.Id == id); 
            var index = items.IndexOf(originalItem);
            items.Insert(index, editedItem);
            return await Task.FromResult((IHttpActionResult)Ok());
        }

        [HttpDelete]
        public async Task<IHttpActionResult> DeleteAsync(Guid id)
        {
            items.Remove(items.Find(item => item.Id == id));
            return await Task.FromResult((IHttpActionResult)Ok());
        }

    }
}
