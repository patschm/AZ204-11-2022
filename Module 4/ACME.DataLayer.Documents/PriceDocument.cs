namespace ACME.DataLayer.Documents;

public class PriceDocument: BaseDocument
{
    public PriceDocument(): base(DocumentType.Price)
    {
    }
    public string? ProductId { get; set; }
    public double BasePrice { get; set; }
    public string? ShopName { get; set; }
    public DateTime PriceDate { get; set; }
   
}
