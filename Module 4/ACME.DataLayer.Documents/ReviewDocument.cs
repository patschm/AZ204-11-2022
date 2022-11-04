namespace ACME.DataLayer.Documents;

public class ReviewDocument: BaseDocument
{
    public ReviewDocument() : base(DocumentType.Review)
    {
    }
    public string? Text { get; set; }
    public byte Score { get; set; }
    public string? ProductId { get; set; }
    public string? ReviewerId { get; set; }
    public int ReviewType { get; set; }
    public DateTime DateBought { get; set; }
    public string? ReviewUrl { get; set; }
    public string? Organization { get; set; }
    
}
