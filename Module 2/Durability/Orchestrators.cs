using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Threading;
using System;

namespace Durability;

public static class Orchestrators
{
    [FunctionName("Chaining")]
    public static async Task<List<string>> RunOrchestratorSerial(
        [OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        var outputs = new List<string>();

        outputs.Add(await context.CallActivityAsync<string>("Function_Hello", "Tokyo"));
        outputs.Add(await context.CallActivityAsync<string>("Function_Hello", "Seattle"));
        outputs.Add(await context.CallActivityAsync<string>("Function_Hello", "London"));

        return outputs;
    }
    [FunctionName("Parallel")]
    public static async Task<List<string>> RunOrchestratorParallel(
        [OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        var tasks = new List<Task<string>>();

        tasks.Add(context.CallActivityAsync<string>("Function_Hello", "Tokyo"));
        tasks.Add(context.CallActivityAsync<string>("Function_Hello", "Seattle"));
        tasks.Add(context.CallActivityAsync<string>("Function_Hello", "London"));

        await Task.WhenAll(tasks);

        return tasks.Select(t => t.Result).ToList();
    }
    [FunctionName("Monitor")]
    public static async Task RunOrchestratorMonitor(
       [OrchestrationTrigger] IDurableOrchestrationContext context,
       ILogger log)
    {
        var t1 = context.CallActivityAsync<string>("Function_Long", "sub");
        var endTime = context.CurrentUtcDateTime.AddSeconds(30);
        while (context.CurrentUtcDateTime < endTime)
        {
            log.LogInformation("Checking task...");
            if (t1.IsCompleted)
            {
                log.LogInformation($"The final result is {t1.Result}");
                break;
            }
            var next = context.CurrentUtcDateTime.AddSeconds(10);
            await context.CreateTimer(next, CancellationToken.None);
        }
    }
    [FunctionName("External")]
    public static async Task RunOrchestratorExternal(
       [OrchestrationTrigger] IDurableOrchestrationContext context,
       ILogger log)
    {
        log.LogInformation("Doing something...");
        log.LogInformation("Wait for approval");
        try
        {
            await context.WaitForExternalEvent("continue", TimeSpan.FromMinutes(5));
            log.LogInformation("Approved");
        }
        catch(TimeoutException)
        {
            log.LogInformation("Timed out!");
        }
    }
}
