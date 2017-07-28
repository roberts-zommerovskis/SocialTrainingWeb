using AutoMapper;
using DataAccess.Mappers;
using Api.Mappers;
using AutoMapper.Configuration;

namespace Api
{
    public class AutoMapperConfig
    {
        public MapperConfiguration Configure()
        {
            var cfg = new MapperConfigurationExpression();
            cfg.AddProfile(new UserToFullUserDtoMapper());
            cfg.AddProfile(new UserToSimpleUserDtoMapper());
            cfg.AddProfile(new UserLanguageToUserLangsuageDtoMapper());
            cfg.AddProfile(new FullUserDtoToUserInfoViewModelMapper());
            cfg.AddProfile(new SimpleUserDtoToSimpleUserInfoViewModelMapper());
            cfg.AddProfile(new UserLanguageDtoToUserLanguageViewModelMapper());
            cfg.AddProfile(new SimpleUserInfoViewModelToFullUserDtoMapper());
            var config = new MapperConfiguration(cfg);
            Mapper.Initialize(cfg);
            return config;
        }
    }
}