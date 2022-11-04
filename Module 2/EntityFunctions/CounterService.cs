using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFunctions;

public interface ICounterService
{
    Task<int> GetValAsync();
    void Increment(int val = 1);
    void Reset();
}

[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
public class CounterService : ICounterService
{
    [JsonProperty("value")]
    private int data = 0;

    public Task<int> GetValAsync() => Task.FromResult(this.data);

    public void Increment(int val = 1)
    {
        data += val;
    }
    public void Reset()
    {
        data = 0;
    }

    [FunctionName(nameof(CounterService))]
    public static Task Run([EntityTrigger] IDurableEntityContext ctx)
        => ctx.DispatchAsync<CounterService>();
}

