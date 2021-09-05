using DigitalLibrary.Models.BookModels;
using DigitalLibrary.Models.Entities;
using Profile = AutoMapper.Profile;

namespace DigitalLibrary.API.Profiles
{
    public class GenresProfile : Profile
    {
        public GenresProfile()
        {
            CreateMap<Genre, GenreDto>();
            CreateMap<GenreForCreationDto, Genre>();
        }
    }
}
