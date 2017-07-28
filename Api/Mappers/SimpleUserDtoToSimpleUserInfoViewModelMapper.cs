using AutoMapper;
using DataAccess.Dto;
using Api.Models;

namespace Api.Mappers
{
    public class SimpleUserDtoToSimpleUserInfoViewModelMapper : Profile
    {
        public SimpleUserDtoToSimpleUserInfoViewModelMapper()
        {
            CreateMap<SimpleUserDto, SimpleUserInfoViewModel>().ReverseMap();
        }
    }
}