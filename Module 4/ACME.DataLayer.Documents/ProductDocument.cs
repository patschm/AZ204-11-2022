namespace ACME.DataLayer.Documents;

public class ProductDocument : BaseDocument 
{
    public ProductDocument(): base(DocumentType.Product)
    {
    }
    public string? Name { get; set; }
    public string? BrandId { get; set; }
    public string? ProductGroupId { get; set; }
    public string? Image { get; set; }
}