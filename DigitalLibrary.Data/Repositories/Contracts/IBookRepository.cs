using System;
using System.Collections;
using System.Collections.Generic;
using DigitalLibrary.Models.Entities;
using DigitalLibrary.Models.Helpers;
using DigitalLibrary.Models.Parameters;

namespace DigitalLibrary.Data.Contracts.Repositories
{
    public interface IBookRepository : IRepository<Book>
    {
        PagedList<Book> GetBooks(BookParameters bookParameters);
        Book GetBookById(Guid bookId);
        void CreateBook(Book book);
        void UpdateBook(Book dbBook, Book book);
        void DeleteBook(Book book);

        void AddGenre(Guid bookId, Guid genreId);
        void AddSubject(Guid bookId, Guid subjectId);
        void AddAuthor(Guid bookId, Guid authorId);
        void AddPublisher(Guid bookId, Guid publisherId);
    }
}
