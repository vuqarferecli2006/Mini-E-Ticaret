using AutoMapper;
using E_Biznes.Application.DTOs.CategoryDtos;
using E_Biznes.Domain.Entities;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<CategoryMainCreateDto, Category>()
            .ForMember(dest => dest.ParentCategoryId, opt => opt.Ignore());
        
        CreateMap<Category, CategoryMainGetDto>()
            .ForMember(dest => dest.SubCategories, opt => opt.MapFrom(src => src.SubCategories));

        

        CreateMap<CategoryMainUpdateDto, Category>()
            .ForMember(dest => dest.ParentCategoryId, opt => opt.Ignore());

        CreateMap<Category, CategorySubGetDto>()
            .ForMember(dest => dest.SubCategories, opt => opt.MapFrom(src => src.SubCategories));

        CreateMap<CategorySubUpdateDto, Category>();

        CreateMap<CategorySubCreateDto, Category>();
        
        CreateMap<CategoryUpdateDto, Category>();

        CreateMap<Category, CategoryUpdateDto>();

    }
}



