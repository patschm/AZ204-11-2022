using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;

namespace BasicFunction
{
    public class FunctionWithDI
    {
        private readonly HttpClient _client;

        public FunctionWithDI(HttpClient client)
        {
            _client = client;
        }

        [FunctionName("FunctionWithDI")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            _client.BaseAddress = new Uri("https://nu.nl/");
            var resp = await _client.GetStringAsync("rss");

            return new OkObjectResult(resp);
        }
    }
}
