using AutoMapper;
using E_Biznes.Application.DTOs.ReviewDtos;
using E_Biznes.Domain.Entities;

namespace E_Biznes.Application.Mappings.ReviewMappingProfile;

public class ReviewProfile : Profile
{
    public ReviewProfile()
    {

        CreateMap<Review, ReviewGetDto>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : "Unknown"));
        CreateMap<Review, ReviewUserGetDto>()
           .ForMember(dest => dest.ReviewId, opt => opt.MapFrom(src => src.Id))
           .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
           .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src =>
               src.Product.ProductImages.FirstOrDefault(i => i.IsMain) != null
                   ? src.Product.ProductImages.First(i => i.IsMain).Image_Url
                   : string.Empty));
    }
}

