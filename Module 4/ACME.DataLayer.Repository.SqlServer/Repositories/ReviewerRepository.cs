using ACME.DataLayer.Entities;
using ACME.DataLayer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ACME.DataLayer.Repository.SqlServer;

public class ReviewerRepository : BaseRepository<Reviewer>, IReviewerRepository
{
    private readonly ShopDatabaseContext _context;

    public ReviewerRepository(ShopDatabaseContext context): base(context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Review>> GetReviewsAsync(long reviewerid, int page = 1, int count = 10)
    {
        return await Context.Reviews
            .Where(r => r.ReviewerId == reviewerid)
            .Skip((page - 1) * count)
            .Take(count)
            .ToListAsync();
    }
    public async Task<IEnumerable<ConsumerReview>> GetConsumerReviewsAsync(long reviewerid, int page = 1, int count = 10)
    {
        return await Context.ConsumerReviews
            .Where(r=>r.ReviewerId == reviewerid)
            .Skip((page - 1) * count)
            .Take(count)
            .ToListAsync();
    }
    public async Task<IEnumerable<ExpertReview>> GetExpertReviewsAsync(long reviewerid, int page = 1, int count = 10)
    {
        return await Context.ExpertReviews
            .Where(r => r.ReviewerId == reviewerid)
            .Skip((page - 1) * count)
            .Take(count)
            .ToListAsync();
    }
    public async Task<IEnumerable<WebReview>> GetWebReviewsAsync(long reviewerid, int page = 1, int count = 10)
    {
        return await Context.WebReviews
            .Where(r => r.ReviewerId == reviewerid)
            .Skip((page - 1) * count)
            .Take(count)
            .ToListAsync();
    }
}
