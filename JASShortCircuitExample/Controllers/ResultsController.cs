using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JASShortCircuitExample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultsController : ControllerBase
    {
        // POST: api/Results
        [HttpPost]
        [TypeFilter(typeof(ResultsResourceFilter), Arguments = new object[] { "jas.api_product_post" })]
        public void Post() {}
    }
}