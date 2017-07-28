using Api.Models;
using AutoMapper;
using DataAccess.Dto;

namespace Api.Mappers
{
    public class SimpleUserInfoViewModelToFullUserDtoMapper : Profile
    {
        public SimpleUserInfoViewModelToFullUserDtoMapper()
        {
            CreateMap<SimpleUserInfoViewModel, FullUserDto>()
                .ForMember(d => d.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(d => d.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.KarmaPoints, opt => opt.MapFrom(src => src.KarmaPoints))
                .ForMember(d => d.ImportId, opt => opt.MapFrom(src => src.ImportId))
                .ForMember(d => d.Aura, opt => opt.MapFrom(src => src.Aura))
                .ForMember(d => d.Languages, opt => opt.Ignore())
                .ForMember(d => d.Team, opt => opt.Ignore())
                .ForMember(d => d.Project, opt => opt.Ignore())
                .ForMember(d => d.Country, opt => opt.Ignore())
                .ForMember(d => d.PrimaryClientContact, opt => opt.Ignore())
                .ForMember(d => d.SecondaryClientContact, opt => opt.Ignore())
                .ReverseMap();
                
        }
    }
}