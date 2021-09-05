namespace DigitalLibrary.Data.Contracts.Repositories
{
    public interface IRepositoryWrapper
    {
        IBookRepository Book { get; }
        ISubjectRepository Subject { get; }
        IGenreRepository Genre { get; }
        IAuthorRepository Author { get; }
        IStorageRepository Storage { get; }
        IProfileRepository Profile { get; }
        IAccountingRepository Accounting { get; }
        ILibraryRepository Library { get; }
        IPublisherRepository Publisher { get; }
        void Save();
    }
}
