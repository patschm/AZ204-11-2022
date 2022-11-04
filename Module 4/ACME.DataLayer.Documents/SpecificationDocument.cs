namespace ACME.DataLayer.Documents;

public class SpecificationDocument : BaseDocument
{
	public SpecificationDocument(): base(DocumentType.Specification)
	{
	}
    public string? Key { get; set; }
    public string? Name { get; set; }
    public string? Unit { get; set; }
    public string? DataType { get; set; }
    public string? Description { get; set; }
    public string? ProductGroupId { get; set; }
    public bool? BoolValue { get; set; }
    public string? StringValue { get; set; }
    public double? NumberValue { get; set; }
    public string[]? ArrayValues { get; set; }
    public string? ProductId { get; set; }

}
