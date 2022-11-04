using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace SomeFunction;

public class Timers
{
    [FunctionName("Starter")]
    public void Bla([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, ILogger log)
    {
        log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
    }

    [FunctionName("Httpers")]
    public static IActionResult Run(
       [HttpTrigger(AuthorizationLevel.Function, "get", Route = "pat/{name}/{age}")] HttpRequest req,
       string name,
       int age,
       ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");


        var responseMessage = new
        {
            Name = name,
            Age = age
        };

        return new OkObjectResult(responseMessage);
    }
}
