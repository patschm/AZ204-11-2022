using ACME.DataLayer.Documents.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ACME.DataLayer.Documents;

public class BaseDocument
{
    protected BaseDocument(string type)
    {
        Type = type;
        PartitionKey = type;
        InternalId = Guid.NewGuid().ToString();
    }

    [JsonProperty(PropertyName = "id")]
    public string InternalId { get; set; }
    public long Id { 
        get=>InternalId.ToPrimaryKey(); 
        set=>InternalId=value.ToDocumentId(Type); 
    }
    public string? Type { get; set; }
    [JsonProperty("partitionkey")]
    public string? PartitionKey { get; set; }

    [JsonExtensionData]
    public IDictionary<string, JToken>? UnmappedData { get; set; }
}
