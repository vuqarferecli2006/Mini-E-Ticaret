using AutoMapper;
using E_Biznes.Application.DTOs.CategoryDtos;
using E_Biznes.Domain.Entities;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<CategoryMainCreateDto, Category>()
           .ForMember(dest => dest.ParentCategoryId, opt => opt.Ignore());

        CreateMap<CategorySubCreateDto, Category>();

        CreateMap<Category, CategoryMainGetDto>()
            .ForMember(dest => dest.SubCategories,
                opt => opt.MapFrom(src => src.SubCategories));

        CreateMap<Category, CategorySubGetDto>();

        CreateMap<Category, CategoryUpdateDto>();
    }
}

