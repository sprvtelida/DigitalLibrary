using DigitalLibrary.Models.Entities;
using DigitalLibrary.Models.StorageModels;
using Profile = AutoMapper.Profile;

namespace DigitalLibrary.API.Profiles
{
    public class StorageProfile : Profile
    {
        public StorageProfile()
        {
            CreateMap<BookItem, StoredItemDto>().ReverseMap();
        }
    }
}
