using System;
using System.Collections.Generic;
using System.Linq;
using DigitalLibrary.Data.Contracts.Repositories;
using DigitalLibrary.Models.Entities;
using DigitalLibrary.Models.Helpers;
using DigitalLibrary.Models.Parameters;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;
using IdentityServer4.Extensions;

namespace DigitalLibrary.Data.Repositories
{
    public class BookRepository : Repository<Book>, IBookRepository
    {
        private ISortHelper<Book> _sortHelper;

        public BookRepository(AppDbContext appDbContext, ISortHelper<Book> sortHelper)
            : base(appDbContext)
        {
            _sortHelper = sortHelper;
        }

        public PagedList<Book> GetBooks(BookParameters bookParameters)
        {
            var books = FindByCondition(book => (bookParameters.MinYear <= book.Year && bookParameters.MaxYear >= book.Year));

            if (bookParameters.MaxPages > 0 && bookParameters.MinPages > 0)
            {
                books = books.Where(book => book.Pages <= bookParameters.MaxPages && book.Pages >= bookParameters.MinPages);
            }

            if (bookParameters.MaxPages > 0 || bookParameters.MinPages > 0)
            {
                books = books.Where(book => book.Pages <= bookParameters.MaxPages || book.Pages >= bookParameters.MinPages);
            }

            if (bookParameters.SearchTerm.IsNullOrEmpty() == false)
            {
                if (bookParameters.SearchField.Equals("Title", StringComparison.InvariantCultureIgnoreCase))
                    books = books.Where(book => book.Title.Contains(bookParameters.SearchTerm, StringComparison.InvariantCultureIgnoreCase));
                if (bookParameters.SearchField.Equals("Author", StringComparison.InvariantCultureIgnoreCase))
                {
                    var name = bookParameters.SearchTerm.Split();
                    books = books.Where(book => book.Author.FirstName.Contains(name[0], StringComparison.InvariantCultureIgnoreCase)
                        && book.Author.LastName.Contains(name[1], StringComparison.InvariantCultureIgnoreCase));
                }
                if (bookParameters.SearchField.Equals("ISBN", StringComparison.InvariantCultureIgnoreCase))
                    books = books.Where(book => book.ISBN.Contains(bookParameters.SearchTerm, StringComparison.InvariantCultureIgnoreCase));
            }

            if (bookParameters.LibraryId != null && bookParameters.OnlyInStorage)
            {
                var guidsOfStoredBooks = AppDbContext.Storage.Where(s => s.Library.Id.Equals(new Guid(bookParameters.LibraryId)))
                    .Select(s => s.Book).GroupBy(b => b.Id).Select(g => g.Key)
                    .ToList();

                books = books.Where(book => guidsOfStoredBooks.Contains(book.Id));
            }

            if (bookParameters.GenreIds.Any())
            {
                books = books.Where(book => bookParameters.GenreIds.Contains(book.Genre.Id.ToString()));
            }

            if (bookParameters.SubjectIds.Any())
            {
                books = books.Where(book => bookParameters.SubjectIds.Contains(book.Subject.Id.ToString()));
            }

            books = books.Include(b => b.Publisher)
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .Include(b => b.Subject);

            var sortedBooks = _sortHelper.ApplySort(books, bookParameters.OrderBy);

            return PagedList<Book>.ToPagedList(sortedBooks,
                bookParameters.PageNumber,
                bookParameters.PageSize);
        }

        public Book GetBookById(Guid bookId)
        {
            return FindByCondition(book => book.Id.Equals(bookId))
                .Include(x => x.Subject)
                .Include(x => x.Genre)
                .Include(x => x.Author)
                .Include(x => x.Publisher)
                .FirstOrDefault();
        }

        public void CreateBook(Book book)
        {
            Create(book);
        }

        public void UpdateBook(Book dbBook, Book book)
        {
            dbBook.Author = book.Author;
            dbBook.Description = book.Description;
            dbBook.Epub = book.Epub ?? dbBook.Epub;
            dbBook.Fb2 = book.Fb2 ?? dbBook.Fb2;
            dbBook.Genre = book.Genre;
            dbBook.Id = book.Id;
            dbBook.Image = book.Image ?? dbBook.Image;
            dbBook.Language = book.Language;
            dbBook.Pages = book.Pages;
            dbBook.Pdf = book.Pdf ?? dbBook.Pdf;
            dbBook.Publisher = book.Publisher;
            dbBook.Subject = book.Subject;
            dbBook.Title = book.Title;
            dbBook.Year = book.Year;
            dbBook.ISBN = book.ISBN;
            Update(dbBook);
        }

        public void DeleteBook(Book book)
        {
            Delete(book);
        }

        public void AddGenre(Guid bookId, Guid genreId)
        {
            var genre = AppDbContext.Genres.Find(genreId);
            AppDbContext.Books.Find(bookId).Genre = genre;
        }

        public void AddSubject(Guid bookId, Guid subjectId)
        {
            var subject = AppDbContext.Subjects.Find(subjectId);
            AppDbContext.Books.Find(bookId).Subject = subject;
        }

        public void AddAuthor(Guid bookId, Guid authorId)
        {
            var author = AppDbContext.Authors.Find(authorId);
            AppDbContext.Books.Find(bookId).Author = author;
        }

        public void AddPublisher(Guid bookId, Guid publisherId)
        {
            var publisher = AppDbContext.Publishers.Find(publisherId);
            AppDbContext.Books.Find(bookId).Publisher = publisher;
        }
    }
}
