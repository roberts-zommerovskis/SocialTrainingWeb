using AutoMapper;
using DataAccess.Dto;
using DataModel.DataModel;

namespace DataAccess.Mappers
{
    public class UserToSimpleUserDtoMapper : Profile
    {
        public UserToSimpleUserDtoMapper()
        {
            CreateMap<User, SimpleUserDto>()
                .ForMember(d => d.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(d => d.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.UserId))
                .ForMember(d => d.Aura, opt => opt.MapFrom(src => src.UserKarma == null ? 0 : src.UserKarma.Aura))
                .ForMember(d => d.KarmaPoints, opt => opt.MapFrom(src => src.UserKarma == null ? 0 : src.UserKarma.Current)).ReverseMap();
        }
    }
}

