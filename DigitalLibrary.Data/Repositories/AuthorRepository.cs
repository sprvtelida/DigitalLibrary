using System;
using System.Collections;
using System.Linq;
using DigitalLibrary.Data.Contracts.Repositories;
using DigitalLibrary.Models.Entities;

namespace DigitalLibrary.Data.Repositories
{
    public class AuthorRepository : Repository<Author>, IAuthorRepository
    {
        public AuthorRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        public void GetBooks()
        {
            throw new System.NotImplementedException();
        }

        public Author GetAuthorById(Guid authorId)
        {
            return FindByCondition(author => author.Id.Equals(authorId)).FirstOrDefault();
        }
    }
}
