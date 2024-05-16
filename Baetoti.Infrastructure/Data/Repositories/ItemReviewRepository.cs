using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;

namespace Baetoti.Infrastructure.Data.Repositories
{
    public class ItemReviewRepository : EFRepository<ItemReview>, IItemReviewRepository
    {

        private readonly BaetotiDbContext _dbContext;

        public ItemReviewRepository(BaetotiDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

    }
}
