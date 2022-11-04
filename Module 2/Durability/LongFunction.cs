using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Durability;

public static class LongFunction
{
    [FunctionName("Function_Long")]
    public static async Task<string> RunLong([ActivityTrigger] string name, ILogger log)
    {
        await Task.Delay(30000);
        return $"Finally: {name}";
    }
}
