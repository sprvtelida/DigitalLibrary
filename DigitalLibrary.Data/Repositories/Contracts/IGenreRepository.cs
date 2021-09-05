using System;
using DigitalLibrary.Models.Entities;

namespace DigitalLibrary.Data.Contracts.Repositories
{
    public interface IGenreRepository : IRepository<Genre>
    {
        Genre GetGenreById(Guid genreId);
    }
}
