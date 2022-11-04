using ACME.DataLayer.Documents.Converters;

namespace ACME.DataLayer.Documents;

public class BrandDocument: BaseDocument
{
	public BrandDocument(): base(DocumentType.Brand)	
    { 
    }
	public string? Name { get; set; }
    public string? Website { get; set; }
}
