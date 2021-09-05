using System;
using System.Collections;
using System.Collections.Generic;
using AutoMapper;
using DigitalLibrary.Models.ProfileModels;

namespace DigitalLibrary.API.Profiles
{
    public class ProfilesProfile : Profile
    {
        public ProfilesProfile()
        {
            CreateMap<ProfileForCreationDto, Models.Entities.Profile>()
                .ForMember(dest => dest.Image, opt =>
                    opt.MapFrom(src => new Guid()));


            CreateMap<Models.Entities.Profile, ProfileDto>();
            //CreateMap<IEnumerable<Models.Entities.Profile>, IEnumerable<ProfileDto>>();
        }
    }
}
