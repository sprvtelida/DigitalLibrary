using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DigitalLibrary.Data.Contracts.Repositories;
using DigitalLibrary.Models.Entities;
using DigitalLibrary.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace DigitalLibrary.Data.Repositories
{
    public class AccountingRepository: Repository<Accounting>, IAccountingRepository
    {
        public AccountingRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        public void AttachProfile(Guid accountingId, Guid profileId)
        {
            var accounting = AppDbContext.Accountings.Find(accountingId);
            var profile = AppDbContext.Profiles.Find(profileId);
            if (profile != null && accounting != null)
            {
                accounting.Profile = profile;
            }
        }

        public void AttachStore(Guid accountingId, Guid storeId)
        {
            var accounting = AppDbContext.Accountings.Find(accountingId);
            var store = AppDbContext.Storage.Find(storeId);
            if (store != null && accounting != null)
            {
                accounting.StoredItem = store;
            }
        }

        public void AttachLibrary(Guid accountingId, Guid libraryId)
        {
            var accounting = AppDbContext.Accountings.Find(accountingId);
            var library = AppDbContext.Libraries.Find(libraryId);
            if (library != null && accounting != null)
            {
                accounting.Library = library;
            }
        }

        public void AttachBook(Guid accountingId, Guid bookId)
        {
            var accounting = AppDbContext.Accountings.Find(accountingId);
            var book = AppDbContext.Books.Find(bookId);
            if (book != null && accounting != null)
            {
                accounting.Book = book;
            }
        }

        public IEnumerable<Accounting> GetAccountingsWithInfo()
        {
            return FindAll().Include(x => x.Book)
                .Include(x => x.Library)
                .Include(x => x.Profile)
                .Include(x => x.StoredItem).ToList();
        }

        public IEnumerable<Accounting> GetAccountingsFromLibrary(Guid libraryId)
        {
            return FindAll()
                .Include(x => x.Library)
                .Include(x => x.StoredItem)
                .Include(x => x.Profile)
                .Include(x => x.Book)
                .Where(x => x.Library.Id.Equals(libraryId))
                .Where(x => x.Status != Status.Declined && x.Status != Status.Finished).ToList();
        }

        public IEnumerable<Accounting> GetAccountingsWithUserId(Guid userId)
        {
            return FindAll()
                .Include(x => x.Library)
                .Include(x => x.StoredItem)
                .Include(x => x.Profile)
                .Include(x => x.Book)
                .Where(x => x.Profile.Id == userId).ToList();
        }

        public void CreateWithAttachments(Guid storageId, Guid bookId, Guid libraryId, Guid profileId, Accounting accounting)
        {
            var store = AppDbContext.Storage.Find(storageId);
            var book = AppDbContext.Books.Find(bookId);
            var library = AppDbContext.Libraries.Find(libraryId);
            var profile = AppDbContext.Profiles.Find(profileId);

            accounting.StoredItem = store;
            accounting.Book = book;
            accounting.Library = library;
            accounting.Profile = profile;

            Create(accounting);
        }
    }
}
