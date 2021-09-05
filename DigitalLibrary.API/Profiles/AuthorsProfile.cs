using DigitalLibrary.Models.AuthorModels;
using DigitalLibrary.Models.Entities;
using Profile = AutoMapper.Profile;

namespace DigitalLibrary.API.Profiles
{
    public class AuthorsProfile : Profile
    {
        public AuthorsProfile()
        {
            CreateMap<AuthorForCreationDto, Author>();
            CreateMap<Author, AuthorDto>().ReverseMap();
        }
    }
}
