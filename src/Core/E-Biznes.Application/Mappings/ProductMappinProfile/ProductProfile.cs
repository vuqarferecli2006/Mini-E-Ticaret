using AutoMapper;
using E_Biznes.Application.DTOs;
using E_Biznes.Application.DTOs.ImageDtos;
using E_Biznes.Application.DTOs.ProducDtos;
using E_Biznes.Domain.Entities;

namespace E_Biznes.Application.Mappings.ProductMappinProfile;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<ProductCreateWithImagesDto, Product>()
            .ForMember(dest => dest.ProductImages, opt => opt.Ignore()) // şəkillər manual əlavə olunacaq
            .ForMember(dest => dest.UserId, opt => opt.Ignore());    // Entity özü Id təyin etsin

        CreateMap<ProductUpdateWithImagesDto, Product>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.ProductImages, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore());

        CreateMap<Product, ProductGetDto>()
            .ForMember(dest => dest.Condition, opt => opt.MapFrom(src => src.Condition.ToString()))
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ProductImages));

        CreateMap<Image, ImageDto>();

    }
}
