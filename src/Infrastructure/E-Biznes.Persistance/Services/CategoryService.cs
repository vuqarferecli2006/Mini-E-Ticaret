using AutoMapper;
using E_Biznes.Application.Abstract.Repositories;
using E_Biznes.Application.Abstract.Service;
using E_Biznes.Application.DTOs.CategoryDtos;
using E_Biznes.Application.Shared;
using E_Biznes.Application.Validations.CategoryValidations;
using E_Biznes.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace E_Biznes.Persistance.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<BaseResponse<string>> AddSubCategoryAsync(CategorySubCreateDto dto)
    {
        if (dto.ParentCategoryId == null)
            return new("Subcategory must have a parent category", HttpStatusCode.BadRequest);

        var parentExists = await _categoryRepository
            .GetByFiltered(c => c.Id == dto.ParentCategoryId)
            .AnyAsync();

        if (!parentExists)
            return new("Parent category not found", HttpStatusCode.BadRequest);

        var exists = await _categoryRepository
            .GetByFiltered(c => c.Name.Trim().ToLower() == dto.Name.Trim().ToLower() &&
                                c.ParentCategoryId == dto.ParentCategoryId)
            .AnyAsync();

        if (exists)
            return new("This subcategory already exists under the specified parent", HttpStatusCode.BadRequest);

        var category = _mapper.Map<Category>(dto);
        await _categoryRepository.AddAsync(category);
        await _categoryRepository.SaveChangeAsync();

        return new("Subcategory created",true, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<string>> AddMainCategoryAsync(CategoryMainCreateDto dto)
    {

        var exists = await _categoryRepository
            .GetByFiltered(c => c.Name.Trim().ToLower() == dto.Name.Trim().ToLower())
            .AnyAsync();

        if (exists)
            return new("This main category already exists", HttpStatusCode.BadRequest);

        var category = _mapper.Map<Category>(dto);
        await _categoryRepository.AddAsync(category);
        await _categoryRepository.SaveChangeAsync();

        return new("Main category created",true, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<object>> DeleteAsync(Guid id)
    {

        var categoryDb = await _categoryRepository.GetByIdAsync(id);
        if (categoryDb is null)
            return new("Id not faund", HttpStatusCode.NotFound);

        _categoryRepository.Delete(categoryDb);
        await _categoryRepository.SaveChangeAsync();
        return new("Category is deleted",true, HttpStatusCode.OK);

    }

    public async Task<BaseResponse<List<CategoryMainGetDto>>> GetAllAsync()
    {
        var categories = await _categoryRepository
            .GetAll()
            .Where(c => c.ParentCategoryId == Guid.Empty || c.ParentCategoryId == null)  // yalnız main category-lər
            .Include(c => c.SubCategories) // yalnız bir səviyyə subcategory gətiririk
            .ToListAsync();

        var dtos = _mapper.Map<List<CategoryMainGetDto>>(categories);

        return new("Success", dtos,true, HttpStatusCode.OK);
    }

    public Task<BaseResponse<string>> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<BaseResponse<List<CategoryMainGetDto>>> GetByNameAsync(string search)
    {
        throw new NotImplementedException();
    }

    public Task<BaseResponse<CategoryUpdateDto>> UpdateAsync(Guid? id, CategoryUpdateDto dto)
    {
        throw new NotImplementedException();
    }
}
