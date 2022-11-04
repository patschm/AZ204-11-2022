using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace EntityFunctions;

public static class Orchestrators
{
    [FunctionName("ClassBased")]
    public static async Task RunOrchestratorClass(
        [OrchestrationTrigger] IDurableOrchestrationContext context,
         ILogger log)
    {
        log.LogInformation("Starting ClassBased Orchestrator");
        var entityId = new EntityId(nameof(CounterService), "mijn");

        var svc = context.CreateEntityProxy<ICounterService>(entityId);
       // svc.Reset();
        svc.Increment(5);
        var result = await svc.GetValAsync();
        log.LogInformation($"Class Based: {result}");

        svc.Increment(5);
        result = await svc.GetValAsync();
        log.LogInformation($"Class Based: {result}");
    }

    [FunctionName("FunctionBased")]
    public static async Task RunOrchestratorFunction(
        [OrchestrationTrigger] IDurableOrchestrationContext context,
         ILogger log)
    {
        log.LogInformation("Starting Function Based Orchestrator");
        var entityId = new EntityId(nameof(Counter), "counter");

        var result = await context.CallEntityAsync<int>(entityId, "reset");
        log.LogInformation($"Function Based: {result}");
        result = await context.CallEntityAsync<int>(entityId, "add", 5);
        log.LogInformation($"Function Based: {result}");

        result = await context.CallEntityAsync<int>(entityId, "add", 5);
        log.LogInformation($"Function Based: {result}");
    }

    [FunctionName("Counter")]
    public static void Counter(
        [EntityTrigger] IDurableEntityContext ctx,
        ILogger log)
    {
        if (ctx.OperationName.ToLower() == "add")
        {
            var amount = ctx.GetInput<int>();
            var current = ctx.GetState<int>();
            ctx.SetState(current + amount);
        }
        if (ctx.OperationName.ToLower() == "reset")
        {
            ctx.SetState(0);
        }
        var val = ctx.GetState<int>();
        ctx.Return(val);
    }
}