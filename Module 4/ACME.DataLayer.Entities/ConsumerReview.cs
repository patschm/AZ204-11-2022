
using ACME.DataLayer.Entities;

public class ConsumerReview : Review
{
	public ConsumerReview()
	{
		ReviewType = ReviewType.Consumer;
	}
    public DateTime DateBought { get; set; }
}
