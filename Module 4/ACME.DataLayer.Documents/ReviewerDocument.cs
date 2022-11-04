namespace ACME.DataLayer.Documents;

public class ReviewerDocument: BaseDocument
{
    public ReviewerDocument(): base(DocumentType.Reviewer)
    {
    }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? UserName { get; set; }
    public string? PasswordHash { get; set; }
    public string? PasswordSalt { get; set; }
}
