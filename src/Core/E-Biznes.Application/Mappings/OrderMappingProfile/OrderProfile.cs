using AutoMapper;
using E_Biznes.Application.DTOs.OrderDtos;
using E_Biznes.Application.DTOs.UserDtos;
using E_Biznes.Domain.Entities;

namespace E_Biznes.Application.Mappings.OrderMappingProfile;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<AppUser, UserGetDto>();
       
        CreateMap<Order, OrderGetDto>()
            .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User.FullName))
            .ForMember(dest => dest.OrderProducts, opt => opt.MapFrom(src => src.OrderProducts))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User.FullName));
     
        CreateMap<OrderProduct, OrderProductDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));
  
        CreateMap<OrderProduct, OrderProductDto>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));

        CreateMap<(AppUser user, Product product, int quantity, decimal total), OrderEmailDetailsDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.user.FullName))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.product.Name))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.quantity))
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.product.Price))
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.total));
    }
}
