using AutoMapper;
using PGB.Auth.Domain.Entities;
using PGB.Auth.Application.Queries;

namespace PGB.Auth.Application.Mappings
{
    #region User Mapping Profile
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            // User Entity to UserDto
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UsernameValue))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.EmailValue))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FullName.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.FullName.LastName))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.DisplayName))
                .ForMember(dest => dest.Initials, opt => opt.MapFrom(src => src.Initials));
        }
    }
    #endregion
}