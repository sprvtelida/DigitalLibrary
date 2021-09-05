using System;
using System.Collections;
using System.Collections.Generic;
using DigitalLibrary.Models.Entities;

namespace DigitalLibrary.Data.Contracts.Repositories
{
    public interface IAccountingRepository : IRepository<Accounting>
    {
        void AttachProfile(Guid accountingId, Guid profileId);
        void AttachStore(Guid accountingId, Guid storageId);
        void AttachLibrary(Guid accountingId, Guid libraryId);
        void AttachBook(Guid accountingId, Guid bookId);
        IEnumerable<Accounting> GetAccountingsWithInfo();
        IEnumerable<Accounting> GetAccountingsFromLibrary(Guid libraryId);
        public IEnumerable<Accounting> GetAccountingsWithUserId(Guid userId);

        void CreateWithAttachments(Guid storageId, Guid bookId, Guid libraryId, Guid profileId, Accounting accounting);
    }
}
