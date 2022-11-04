using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Durability;

public static class TriggerFunction
{
    [FunctionName("HttpStarter")]
    public static async Task<HttpResponseMessage> HttpStart(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route ="{option}")] HttpRequestMessage req,
        int option,
        [DurableClient] IDurableOrchestrationClient starter,
        ILogger log)
    {
        string instanceId = "";
        switch(option)
        {
            case 1:
                instanceId = await starter.StartNewAsync("Chaining", "serial");
                break;
            case 2:
                instanceId = await starter.StartNewAsync("Parallel", "parallel");
                break;
            case 3:
                instanceId = await starter.StartNewAsync("Monitor", "monitor");
                break;
            case 4:
                instanceId = await starter.StartNewAsync("External", "external");
                break;
        }
         
        log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

        return starter.CreateCheckStatusResponse(req, instanceId);
    }
}