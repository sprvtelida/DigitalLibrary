using System;
using System.Collections.Generic;
using System.Linq;
using DigitalLibrary.Data.Contracts.Repositories;
using DigitalLibrary.Models.Entities;
using DigitalLibrary.Models.Enums;
using IdentityServer4.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DigitalLibrary.Data.Repositories
{
    public class BookItemRepository : Repository<BookItem>, IStorageRepository
    {
        public BookItemRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        public IEnumerable<Book> GetStoredBooks()
        {
            var books = AppDbContext.Storage.Select(s => s.Book).ToList();
            return books;
        }

        public IEnumerable<BookItem> GetStoredBooks(Guid libraryId)
        {
            var storages = AppDbContext.Storage.Where(s => s.Library.Id.Equals(libraryId))
                .Include(s => s.Book)
                .Include(s => s.Book.Author)
                .ToList();

            return storages;
        }

        public BookItem GetItemByLibraryAndBookId(Guid libraryId, Guid itemId)
        {
            return FindByCondition(s => s.Library.Id.Equals(libraryId) &&
                                        s.Id.Equals(itemId)).FirstOrDefault();
        }

        public IEnumerable<Book> GetStoredBooks(Guid libraryId, Guid bookId)
        {
            var books = AppDbContext.Storage.Where(s => s.Library.Id.Equals(libraryId))
                .Select(s => s.Book).Where(book => book.Id.Equals(bookId))
                .ToList();

            return books;
        }

        public IEnumerable<Guid> GetStoredBooksGuids(Guid libraryId)
        {
            var guids = AppDbContext.Storage.Where(s => s.Library.Id.Equals(libraryId))
                .Select(s => s.Book).GroupBy(b => b.Id).Select(g => g.Key)
                .ToList();

            return guids;
        }

        public bool IsInStorage(Guid bookId, Guid libraryId)
        {
            var storedItems = FindByCondition(storedItem => storedItem.Book.Id.Equals(bookId) && storedItem.Library.Id.Equals(libraryId)).ToList();
            foreach (var storedItem in storedItems)
            {
                var accountings = AppDbContext.Accountings.Where(x => x.StoredItem.Id.Equals(storedItem.Id)).ToList();
                if (!accountings.IsNullOrEmpty())
                {
                    foreach (var accounting in accountings)
                    {
                        if (accounting.Status != Status.Accepted && accounting.Status != Status.Requested)
                            return true;
                    }
                }
                else
                {
                    return true;
                }

            }
            return false;
        }

        public BookItem GetStoreByBookId(Guid bookId)
        {
            return FindByCondition(store => store.Book.Id.Equals(bookId)).FirstOrDefault();
        }

        public void CreateByLibraryIdAndBookId(Guid libraryId, Guid bookId, int quantity = 1)
        {
            var rnd = new Random();
            var library = AppDbContext.Libraries.Find(libraryId);
            var book = AppDbContext.Books.Find(bookId);
            for (int i = 0; i < quantity; i++)
            {
                Create(new BookItem()
                {
                    Book = book,
                    Library = library,
                    ArrivalDate = DateTime.Now,
                    InventoryNumber = rnd.Next(99999)
                });
            }
        }

        public string GetFreeBookItemId(Guid libraryId, Guid bookId)
        {
            var storage = AppDbContext.Storage.Where(
                storage => storage.Book.Id.Equals(bookId) && storage.Library.Id.Equals(libraryId)).ToList();

            foreach (var storageItem in storage)
            {
                if (!AppDbContext.Accountings.Any(acc => acc.StoredItem.Id.Equals(storageItem.Id)))
                {
                    return storageItem.Id.ToString();
                }

                var isUnique = true;
                var accountingsBelongToStoredItem = AppDbContext.Accountings
                    .Where(a => a.StoredItem.Id.Equals(storageItem.Id))
                    .Include(a => a.StoredItem).ToList();

                foreach (var accounting in accountingsBelongToStoredItem)
                {
                    if (accounting.Status == Status.Finished || accounting.Status == Status.Declined && (accounting.Status != Status.Requested || accounting.Status != Status.Accepted))
                    {
                        isUnique = true;
                        break;
                    }
                    isUnique = false;
                }

                if (isUnique)
                {
                    return storageItem.Id.ToString();
                }
            }

            return null;
        }
    }
}
