using DigitalLibrary.Data.Contracts.Repositories;
using DigitalLibrary.Models.Entities;

namespace DigitalLibrary.Data.Repositories
{
    public class LibraryRepository : Repository<Library>, ILibraryRepository
    {
        public LibraryRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}
