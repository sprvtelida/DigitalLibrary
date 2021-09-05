using System;
using DigitalLibrary.Models.Entities;

namespace DigitalLibrary.Data.Contracts.Repositories
{
    public interface IAuthorRepository : IRepository<Author>
    {
        void GetBooks();
        Author GetAuthorById(Guid authorId);
    }
}
