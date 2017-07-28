using System.Linq;
using AutoMapper;
using DataAccess.Dto;
using DataModel.DataModel;

namespace DataAccess.Mappers
{
    public class FullUserDtoToUserMapper : Profile
    {

        public FullUserDtoToUserMapper()
        {
            CreateMap<FullUserDto, User>()
                .ForMember(d => d.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(d => d.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(d => d.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.UserKarma.Current, opt => opt.MapFrom(src => src.KarmaPoints))
                .ForMember(d => d.Project.ProjectTitle, opt => opt.MapFrom(src => src.Project))
                .ForMember(d => d.Team.TeamName, opt => opt.MapFrom(src => src.Team))
                .ForMember(d => d.UserLanguages, opt => opt.ResolveUsing(ResolveUserLanguages))
                .ForMember(d => d.Country, opt => opt.MapFrom(src => src.Country))
                .ForMember(d => d.PrimaryClientContact, opt => opt.MapFrom(src => src.PrimaryClientContact))
                .ForMember(d => d.SecondaryClientContact, opt => opt.MapFrom(src => src.SecondaryClientContact))
                .ForMember(d => d.ImportId, opt => opt.MapFrom(src => src.ImportId))
                .ForMember(d => d.UserKarma.Aura, opt => opt.MapFrom(src => src.Aura));
        }

        private static object ResolveUserLanguages(FullUserDto user)
        {
            return user.Languages.Select(Mapper.Map<UserLanguageDto, UserLanguage>);
        }
    }
}
