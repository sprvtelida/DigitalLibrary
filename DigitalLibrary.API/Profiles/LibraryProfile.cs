using DigitalLibrary.Models.Entities;
using DigitalLibrary.Models.LibraryModels;
using Profile = AutoMapper.Profile;

namespace DigitalLibrary.API.Profiles
{
    public class LibraryProfile : Profile
    {
        public LibraryProfile()
        {
            CreateMap<LibraryForCreationDto, Library>();
            CreateMap<Library, LibraryDto>().ReverseMap();
        }
    }
}
