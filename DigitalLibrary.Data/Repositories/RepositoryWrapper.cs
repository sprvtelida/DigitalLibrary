using DigitalLibrary.Data.Contracts.Repositories;
using DigitalLibrary.Models.Entities;
using DigitalLibrary.Models.Helpers;

namespace DigitalLibrary.Data.Repositories
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private AppDbContext _dbContext;
        private IBookRepository _book;
        private ISortHelper<Book> _bookSortHelper;

        private ISubjectRepository _subject;
        private IGenreRepository _genre;
        private IAuthorRepository _author;
        private IStorageRepository _storage;
        private IProfileRepository _profile;
        private IAccountingRepository _accounting;
        private ILibraryRepository _library;
        private IPublisherRepository _publisher;

        public IBookRepository Book
        {
            get
            {
                if (_book == null)
                {
                    _book = new BookRepository(_dbContext, _bookSortHelper);
                }

                return _book;
            }
        }

        public ISubjectRepository Subject
        {
            get
            {
                if (_subject == null)
                {
                    _subject = new SubjectRepository(_dbContext);
                }

                return _subject;
            }
        }

        public IGenreRepository Genre
        {
            get
            {
                if (_genre == null)
                {
                    _genre = new GenreRepository(_dbContext);
                }

                return _genre;
            }
        }

        public IAuthorRepository Author
        {
            get
            {
                if (_author == null)
                {
                    _author = new AuthorRepository(_dbContext);
                }

                return _author;
            }
        }

        public IStorageRepository Storage
        {
            get
            {
                if (_storage == null)
                {
                    _storage = new BookItemRepository(_dbContext);
                }
                return _storage;
            }
        }

        public IProfileRepository Profile
        {
            get
            {
                if (_profile == null)
                {
                    _profile = new ProfileRepository(_dbContext);
                }
                return _profile;
            }
        }

        public IAccountingRepository Accounting
        {
            get
            {
                if (_accounting == null)
                {
                    _accounting = new AccountingRepository(_dbContext);
                }

                return _accounting;
            }
        }

        public ILibraryRepository Library
        {
            get
            {
                if (_library == null)
                {
                    _library = new LibraryRepository(_dbContext);
                }

                return _library;
            }
        }

        public IPublisherRepository Publisher
        {
            get
            {
                if (_publisher == null)
                {
                    _publisher = new PublisherRepository(_dbContext);
                }

                return _publisher;
            }
        }

        public RepositoryWrapper(
                AppDbContext dbContext,
                ISortHelper<Book> bookSortHelper
            )
        {
            _dbContext = dbContext;
            _bookSortHelper = bookSortHelper;
        }


        public void Save()
        {
            _dbContext.SaveChanges();
        }
    }
}
