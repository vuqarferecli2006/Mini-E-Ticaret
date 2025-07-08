using E_Biznes.Application.DTOs.CategoryDtos;
using E_Biznes.Application.Shared;

namespace E_Biznes.Application.Abstract.Service;

public interface ICategoryService
{
    Task<BaseResponse<string>> AddMainCategoryAsync(CategoryMainCreateDto dto);

    Task<BaseResponse<string>> AddSubCategoryAsync(CategorySubCreateDto dto);

    Task<BaseResponse<CategoryMainGetDto>> GetByIdAsync(Guid id);

    Task<BaseResponse<List<CategoryMainGetDto>>> GetByNameAsync(string search);

    Task<BaseResponse<List<CategoryMainGetDto>>> GetAllAsync();

    Task<BaseResponse<CategoryUpdateDto>> UpdateMainCategoryAsync(CategoryMainUpdateDto dto);

    Task<BaseResponse<CategoryUpdateDto>> UpdateSubCategoryAsync(CategorySubUpdateDto dto);
}
