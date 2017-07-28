using AutoMapper;
using System.Linq;
using DataAccess.Dto;
using DataModel.DataModel;

namespace DataAccess.Mappers
{
    public class UserToFullUserDtoMapper : Profile
    {
        public UserToFullUserDtoMapper()
        {
            CreateMap<User, FullUserDto>()
                .ForMember(d => d.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(d => d.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.UserId))
                .ForMember(d => d.KarmaPoints, opt => opt.MapFrom(src => src.UserKarma == null ? 0 : src.UserKarma.Current))
                .ForMember(d => d.Project, opt => opt.MapFrom(src => src.Project.ProjectTitle))
                .ForMember(d => d.Team, opt => opt.MapFrom(src => src.Team.TeamName))
                .ForMember(d => d.Languages, opt => opt.ResolveUsing(ResolveUserLanguages))
                .ForMember(d => d.Country, opt => opt.MapFrom(src => src.Country))
                .ForMember(d => d.PrimaryClientContact, opt => opt.MapFrom(src => src.PrimaryClientContact))
                .ForMember(d => d.SecondaryClientContact, opt => opt.MapFrom(src => src.SecondaryClientContact))
                .ForMember(d => d.ImportId, opt => opt.MapFrom(src => src.ImportId))
                .ForMember(d => d.Aura, opt => opt.MapFrom(src => src.UserKarma == null ? 0 : src.UserKarma.Aura));
        }

        private static object ResolveUserLanguages(User user)
        {
            return user.UserLanguages.Select(Mapper.Map<UserLanguage, UserLanguageDto>);
        }
    }
}
