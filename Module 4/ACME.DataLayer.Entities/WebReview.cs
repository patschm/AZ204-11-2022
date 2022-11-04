
using ACME.DataLayer.Entities;

public class WebReview : Review
{
	public WebReview()
	{
		ReviewType = ReviewType.Web;
	}
    public string? ReviewUrl { get; set; }
}
