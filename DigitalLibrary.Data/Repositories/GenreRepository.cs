using System;
using System.Linq;
using DigitalLibrary.Data.Contracts.Repositories;
using DigitalLibrary.Models.Entities;

namespace DigitalLibrary.Data.Repositories
{
    public class GenreRepository : Repository<Genre>, IGenreRepository
    {
        public GenreRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        public Genre GetGenreById(Guid genreId)
        {
            return FindByCondition(genre => genre.Id.Equals(genreId))
                .FirstOrDefault();
        }
    }
}
