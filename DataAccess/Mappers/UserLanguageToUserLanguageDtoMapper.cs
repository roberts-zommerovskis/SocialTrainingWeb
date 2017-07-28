using AutoMapper;
using DataAccess.Dto;
using DataModel.DataModel;

namespace DataAccess.Mappers
{
    public class UserLanguageToUserLangsuageDtoMapper : Profile
    {
        public UserLanguageToUserLangsuageDtoMapper()
        {
            CreateMap<UserLanguage, UserLanguageDto>()
                .ForMember(d => d.LanguageId, opt => opt.MapFrom(src => src.LanguageId))
                .ForMember(d => d.LanguageName, opt => opt.MapFrom(src => src.Language.LanguageName)).ReverseMap();

        }
    }
}
