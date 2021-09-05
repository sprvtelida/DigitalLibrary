using DigitalLibrary.Models.Entities;
using DigitalLibrary.Models.BookModels;
using AutoMapper;
using Profile = AutoMapper.Profile;

namespace DigitalLibrary.API.Profiles
{
    public class SubjectsProfile : Profile
    {
        public SubjectsProfile()
        {
            CreateMap<Subject, SubjectDto>();
            CreateMap<SubjectForCreationDto, Subject>();
        }
    }
}
