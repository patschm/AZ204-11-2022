namespace ACME.DataLayer.Documents;

public class ProductGroupDocument: BaseDocument
{
    public ProductGroupDocument(): base(DocumentType.ProductGroup)
    {
    }
    public string? Name { get; set; }
    public string? Image { get; set; }
}
