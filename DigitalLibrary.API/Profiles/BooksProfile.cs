using DigitalLibrary.Models.BookModels;
using DigitalLibrary.Models.Entities;
using Profile = AutoMapper.Profile;

namespace DigitalLibrary.API.Profiles
{
    public class BooksProfile : Profile
    {
        public BooksProfile()
        {
            CreateMap<Book, BookDto>();
            CreateMap<BookForCreationDto, Book>();
            CreateMap<BookForUpdateDto, Book>();
        }
    }
}
