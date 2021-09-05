using DigitalLibrary.Data.Contracts.Repositories;
using DigitalLibrary.Models.Entities;

namespace DigitalLibrary.Data.Repositories
{
    public class PublisherRepository : Repository<Publisher>, IPublisherRepository
    {
        public PublisherRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}
