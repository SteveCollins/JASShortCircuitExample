using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace JASShortCircuitExample
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ResultsResourceFilter : Attribute, IAsyncResourceFilter
    {
        private readonly IConfiguration _config;
        private readonly string proc;

        public ResultsResourceFilter(IConfiguration configuration, object procName)
        {
            _config = configuration;
            proc = procName.ToString();
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            var request = context.HttpContext.Request;

            using (var reader = new StreamReader(request.Body))
            {
                var body = await reader.ReadToEndAsync();

                if (request.Method == "POST")
                {
                    var dal = new DataAccessLayer();
                    var queryResult = await dal.SqlPostAsync(proc, _config.GetConnectionString("JsonAutoServiceExample"), body);
                    if (queryResult)
                    {
                        context.Result = new ContentResult
                        {
                            ContentType = "application/json",
                            StatusCode = StatusCodes.Status201Created
                        };
                    }
                    else
                    {
                        context.Result = new ContentResult
                        {
                            StatusCode = StatusCodes.Status400BadRequest
                        };
                    };
                }
                else
                {
                    context.Result = new ContentResult
                    {
                        StatusCode = StatusCodes.Status405MethodNotAllowed
                    };
                }
            }
        }
    }
}
