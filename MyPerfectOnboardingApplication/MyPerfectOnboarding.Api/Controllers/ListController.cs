using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace MyPerfectOnboarding.Api.Controllers
{
    public class ListController : ApiController
    {

        public async Task<IHttpActionResult> GetAsync()
        {
            var items = new[] {"aaaa", "lll"};
            return await Task.FromResult((IHttpActionResult)Ok(items));
        }
    }
}
