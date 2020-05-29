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
        private readonly IDataAccessLayer _dal;
        private readonly string proc;

        public ResultsResourceFilter(IConfiguration configuration, IDataAccessLayer dataAccessLayer, object procName)
        {
            _config = configuration;
            _dal = dataAccessLayer;
            proc = procName.ToString();
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            var request = context.HttpContext.Request;

            using (var reader = new StreamReader(request.Body))
            {
                var body = await reader.ReadToEndAsync();

                var queryResult = await _dal.SqlPostAsync(proc, _config.GetConnectionString("JsonAutoServiceExample"), body);
                if (queryResult)
                    context.Result = new OkResult();
                else
                    context.Result = new BadRequestResult();
                    //await next();
            }
        }
    }
}
