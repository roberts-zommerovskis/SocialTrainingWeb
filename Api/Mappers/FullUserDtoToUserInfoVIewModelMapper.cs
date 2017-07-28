using System.Linq;
using AutoMapper;
using DataAccess.Dto;
using Api.Models;

namespace Api.Mappers
{
    public class FullUserDtoToUserInfoViewModelMapper : Profile
    {
        public FullUserDtoToUserInfoViewModelMapper()
        {
            CreateMap<FullUserDto, UserInfoViewModel>()
                 .ForMember(d => d.FullName, opt => opt.MapFrom(src => src.FullName))
                      .ForMember(d => d.Email, opt => opt.MapFrom(src => src.Email))
                      .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                      .ForMember(d => d.KarmaPoints, opt => opt.MapFrom(src => src.KarmaPoints))
                      .ForMember(d => d.Project, opt => opt.MapFrom(src => src.Project))
                      .ForMember(d => d.Team, opt => opt.MapFrom(src => src.Team))
                      .ForMember(d => d.Country, opt => opt.MapFrom(src => src.Country))
                      .ForMember(d => d.PrimaryClientContact, opt => opt.MapFrom(src => src.PrimaryClientContact))
                      .ForMember(d => d.SecondaryClientContact, opt => opt.MapFrom(src => src.SecondaryClientContact))
                      .ForMember(d => d.ImportId, opt => opt.MapFrom(src => src.ImportId))
                      .ForMember(d => d.Aura, opt => opt.MapFrom(src => src.Aura))
                      .ForMember(d => d.Languages, opt => opt.ResolveUsing(ResolveUserLanguages)).ReverseMap();
        }

        private static object ResolveUserLanguages(FullUserDto src)
        {
            return src.Languages.Select(Mapper.Map<UserLanguageDto, UserLanguageViewModel>);
        }
    }
}