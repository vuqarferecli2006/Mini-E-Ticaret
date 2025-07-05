using AutoMapper;
using E_Biznes.Application.DTOs.AccountsDto;
using E_Biznes.Application.DTOs.OrderDtos;
using E_Biznes.Application.DTOs.UserDtos;
using E_Biznes.Domain.Entities;

namespace E_Biznes.Application.Mappings.UserMappingProfile;

public class UserRegisterProfile: Profile
{
    public UserRegisterProfile()
    {
        CreateMap<UserRegisterDto, AppUser>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.Age))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
        CreateMap<AccountRegisterDto, AppUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.Age))
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
        CreateMap<AppUser, UserGetDto>();
        CreateMap<Order, OrderDto>();
    }
}
