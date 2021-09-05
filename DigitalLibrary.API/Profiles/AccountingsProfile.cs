using DigitalLibrary.Models.AccountingModels;
using DigitalLibrary.Models.Entities;
using Profile = AutoMapper.Profile;

namespace DigitalLibrary.API.Profiles
{
    public class AccountingsProfile : Profile
    {
        public AccountingsProfile()
        {
            CreateMap<AccountingForCreationDto, Accounting>();
            CreateMap<Accounting, AccountingDto>().ReverseMap();
        }
    }
}
