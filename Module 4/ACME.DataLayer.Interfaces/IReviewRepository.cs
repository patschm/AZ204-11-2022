using ACME.DataLayer.Entities;

namespace ACME.DataLayer.Interfaces;

public interface IReviewRepository : IRepository<Review>
{
    Task<IEnumerable<ExpertReview>> GetExpertReviewsAsync(int page = 1, int count = 10);
    Task<IEnumerable<ConsumerReview>> GetConsumerReviewsAsync(int page = 1, int count = 10);
    Task<IEnumerable<WebReview>> GetWebReviewsAsync(int page = 1, int count = 10);
}
