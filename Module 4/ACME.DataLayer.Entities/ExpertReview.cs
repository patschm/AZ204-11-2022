
using ACME.DataLayer.Entities;

public class ExpertReview : Review
{
	public ExpertReview()
	{
		ReviewType = ReviewType.Expert;
	}
    public string? Organization { get; set; }
}
