using AutoMapper;
using DataAccess.Dto;
using Api.Models;

namespace Api.Mappers
{
    public class UserLanguageDtoToUserLanguageViewModelMapper : Profile
    {
        public UserLanguageDtoToUserLanguageViewModelMapper()
        {
            CreateMap<UserLanguageDto, UserLanguageViewModel>().ReverseMap();
        }
    }
}