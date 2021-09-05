using DigitalLibrary.Models.Entities;
using DigitalLibrary.Models.PublisherModels;
using Profile = AutoMapper.Profile;

namespace DigitalLibrary.API.Profiles
{
    public class PublisherProfile : Profile
    {
        public PublisherProfile()
        {
            CreateMap<PublisherForCreationDto, Publisher>();
            CreateMap<Publisher, PublisherDto>().ReverseMap();
        }
    }
}
