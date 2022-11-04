using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using System.Linq;

// az group create -n az204 -l westeurope
// az storage account create -n psstoredemo -g az204 -l westeurope --sku Standard_LRS
// az storage queue create -n psqueue --account-name psstoredemo
// az storage table create -n pstable --account-name psstoredemo
// "OnlineStorage" in local.settings.json
namespace BasicFunction
{
    public static class StorageFunction
    {
        [FunctionName("StorageFunction")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route =null)] HttpRequest req,
            [Queue("psqueue"), StorageAccount("OnlineStorage")] ICollector<string> queue,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var name = req.Query["name"].First();
            var age = req.Query["age"].First();
            var responseMessage = new { Name = name, Age = age };

            queue.Add(JsonConvert.SerializeObject(responseMessage));

            return new OkObjectResult(responseMessage);
        }
    }
}
