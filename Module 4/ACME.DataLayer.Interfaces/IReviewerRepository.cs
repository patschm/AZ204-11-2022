using ACME.DataLayer.Entities;

namespace ACME.DataLayer.Interfaces;

public interface IReviewerRepository : IRepository<Reviewer>
{
    Task<IEnumerable<Review>> GetReviewsAsync(long reviewerid, int page = 1, int count = 10);
    Task<IEnumerable<ExpertReview>> GetExpertReviewsAsync(long reviewerid, int page = 1, int count = 10);
    Task<IEnumerable<ConsumerReview>> GetConsumerReviewsAsync(long reviewerid, int page = 1, int count = 10);
    Task<IEnumerable<WebReview>> GetWebReviewsAsync(long reviewerid, int page = 1, int count = 10);
}
