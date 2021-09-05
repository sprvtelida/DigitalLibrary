using System;
using System.Collections;
using System.Collections.Generic;
using DigitalLibrary.Models.Entities;

namespace DigitalLibrary.Data.Contracts.Repositories
{
    public interface IStorageRepository : IRepository<BookItem>
    {
        /// <summary>
        /// Gets all stored books from all libraries
        /// </summary>
        /// <returns></returns>
        IEnumerable<Book> GetStoredBooks();
        /// <summary>
        /// Gets all stored books storedItems from particular library
        /// </summary>
        /// <param name="libraryId"></param>
        /// <returns></returns>
        IEnumerable<BookItem> GetStoredBooks(Guid libraryId);

        BookItem GetItemByLibraryAndBookId(Guid libraryId, Guid itemId);
        /// <summary>
        /// Gets particular books with bookId from particular library
        /// </summary>
        /// <param name="libraryId"></param>
        /// <param name="BookId"></param>
        /// <returns></returns>
        IEnumerable<Book> GetStoredBooks(Guid libraryId, Guid bookId);

        /// <summary>
        /// Gets Guids of books that stored in library
        /// </summary>
        /// <param name="libraryId"></param>
        /// <returns></returns>
        IEnumerable<Guid> GetStoredBooksGuids(Guid libraryId);

        bool IsInStorage(Guid bookId, Guid libraryId);

        BookItem GetStoreByBookId(Guid bookId);
        void CreateByLibraryIdAndBookId(Guid libraryId, Guid bookId, int quantity);
        string GetFreeBookItemId(Guid libraryId, Guid bookId);
    }
}
