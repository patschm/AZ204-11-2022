using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

// az group create -n az204 -l westeurope
// az storage account create -n psstoredemo -g az204 -l westeurope --sku Standard_LRS
// az storage queue create -n psqueue --account-name psstoredemo
// az storage table create -n pstable --account-name psstoredemo
namespace BasicFunction;

public class QueueFunction
{
    [FunctionName("QueueFunction")]
    [return: Table("pstable", Connection = "OnlineStorage")] // OnlineStorage in local.settings.json
    public Person Run([QueueTrigger("psqueue", Connection = "OnlineStorage")]string queueItem, ILogger log)
    {
        log.LogInformation($"C# Queue trigger function processed: {queueItem}");
        var p = JsonConvert.DeserializeObject<Person>(queueItem);
        log.LogInformation($"{p.Name} {p.Age}");
        p.PartitionKey = "incoming";
        p.RowKey = Guid.NewGuid().ToString();
        return p;
    }
}
