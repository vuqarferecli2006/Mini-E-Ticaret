﻿using AutoMapper;
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
        if (dto.ParentCategoryId == Guid.Empty)// subcategory yaratmaq üçün parent kateqoriyası olmalıdır
            return new("Subcategory must have a parent category", HttpStatusCode.BadRequest);

        var parentExists = await _categoryRepository//Repository-de yazdigimiz methoddan istifade etdim
            .GetByFiltered(c => c.Id == dto.ParentCategoryId)//category-nin parent kateqoriyasini tapmaq ucun
            .AnyAsync();//true ve ya false qaytarir

        if (!parentExists)
            return new("Parent category not found", HttpStatusCode.BadRequest);

        var exists = await _categoryRepository
            .GetByFiltered(c => c.Name.Trim().ToLower() == dto.Name.Trim().ToLower() &&
                                c.ParentCategoryId == dto.ParentCategoryId).AnyAsync();//bu adda categoriyanin olub olmadigini yoxlayir
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
            .GetByFiltered(c => c.Name.Trim().ToLower() == dto.Name.Trim().ToLower()).AnyAsync();//categorynin olub-olmamasini yoxlayir

        if (exists)
            return new("This main category already exists", HttpStatusCode.BadRequest);

        var category = _mapper.Map<Category>(dto);
        await _categoryRepository.AddAsync(category);
        await _categoryRepository.SaveChangeAsync();

        return new("Main category created",true, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<List<CategoryMainGetDto>>> GetAllAsync()
    {
        var categories = await _categoryRepository
            .GetAll()
            .Where(c => c.ParentCategoryId == Guid.Empty || (c.ParentCategoryId == null&&!c.IsDeleted))//parnecategoryid guid empty-se bu o demekdirki bu main categorydir
            .Include(c => c.SubCategories.Where(sc=>!sc.IsDeleted)) //ve hemin tapilan maincategory-e relation ile bagli olan subcategorsyleri getirir
                .ThenInclude(s=>s.SubCategories.Where(sc => !sc.IsDeleted))
            .ToListAsync();//List kimi qaytarir

        var dtos = _mapper.Map<List<CategoryMainGetDto>>(categories);

        return new("Success", dtos,true, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<CategoryMainGetDto>> GetByIdAsync(Guid id)
    {
        if (id == Guid.Empty)//id bos gelirse
            return new("Id mustn't be empty",HttpStatusCode.BadRequest);

        var category = await _categoryRepository
            .GetByFiltered(c => c.Id == id && c.ParentCategoryId == null)//Burada yoxlanilir ki hemin id-de category var ve main categordirse
            .Include(c => c.SubCategories.Where(sc=>!sc.IsDeleted))//hemin id-e uygun subcategory-i getir
                 .ThenInclude(s => s.SubCategories.Where(sc => !sc.IsDeleted))
            .Where(predicate: c => !c.IsDeleted) //ana categoriya silinmemis olmalidir
            .FirstOrDefaultAsync();//null ve ya data qaytaracaq

        if (category is null)
            return new("Main category not found", false, HttpStatusCode.NotFound);

        var dto = _mapper.Map<CategoryMainGetDto>(category);
        return new (dto, true, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<List<CategoryMainGetDto>>> GetByNameAsync(string search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return new("Search term cannot be empty", false, HttpStatusCode.BadRequest);

        var categories = await _categoryRepository
            .GetAll(isTracking: false)
            .Where(c => c.Name.ToLower().Contains(search.Trim().ToLower())&&!c.IsDeleted)
            .Include(c => c.SubCategories.Where(sc => !sc.IsDeleted))
                 .ThenInclude(s => s.SubCategories.Where(sc => !sc.IsDeleted))
            .ToListAsync();

        if (!categories.Any())
            return new("No categories found matching the search criteria", false, HttpStatusCode.NotFound);

        var mapped = _mapper.Map<List<CategoryMainGetDto>>(categories);
        return new BaseResponse<List<CategoryMainGetDto>>(mapped, true, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<CategoryUpdateDto>> UpdateMainCategoryAsync(CategoryMainUpdateDto dto)
    {
        var category = await _categoryRepository.GetByIdAsync(dto.Id);
        if (category is null)
            return new("Main category not found", false, HttpStatusCode.NotFound);

        if (category.ParentCategoryId != null)
            return new("This is not a main category", false, HttpStatusCode.BadRequest);

        var nameExists = await _categoryRepository.GetByFiltered(c =>
            c.Id != dto.Id &&//Dot-dan gele id-de category olub olmamasi yoxlanir
            c.Name.ToLower().Trim() == dto.Name.ToLower().Trim() &&
            c.ParentCategoryId == null//eger maincategorydirse
        ).AnyAsync();

        if (nameExists)
            return new("Another main category with this name already exists", false, HttpStatusCode.BadRequest);

        _mapper.Map(dto, category); // AutoMapper vasitəsilə yenilənir

        _categoryRepository.Update(category);
        await _categoryRepository.SaveChangeAsync();

        var updatedDto = _mapper.Map<CategoryUpdateDto>(category);
        return new(updatedDto, true, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<CategoryUpdateDto>> UpdateSubCategoryAsync(CategorySubUpdateDto dto)
    {
        var category = await _categoryRepository.GetByIdAsync(dto.Id);
        if (category is null)
            return new("Subcategory not found", false, HttpStatusCode.NotFound);

        if (category.ParentCategoryId == null)
            return new("This is not a subcategory", false, HttpStatusCode.BadRequest);

        if (dto.NewParentCategoryId == dto.Id)
            return new("A category cannot be its own parent", false, HttpStatusCode.BadRequest);

        var parentExists = await _categoryRepository
            .GetByFiltered(c => c.Id == dto.NewParentCategoryId && c.ParentCategoryId == null)
            .AnyAsync();

        if (!parentExists)
            return new("New parent category not found", false, HttpStatusCode.BadRequest);

        var nameExists = await _categoryRepository.GetByFiltered(c =>
            c.Id != dto.Id &&//bu id-de category varmi
            c.Name.ToLower().Trim() == dto.Name.ToLower().Trim() &&//bu adda category varmi
            c.ParentCategoryId == dto.NewParentCategoryId).AnyAsync();//bu id-de parentcategory varmi

        if (nameExists)
            return new("Another subcategory with this name already exists under the selected parent", false, HttpStatusCode.BadRequest);

        _mapper.Map(dto, category);
        category.ParentCategoryId = dto.NewParentCategoryId;

        _categoryRepository.Update(category);
        await _categoryRepository.SaveChangeAsync();

        var updatedDto = _mapper.Map<CategoryUpdateDto>(category);
        return new(updatedDto, true, HttpStatusCode.OK);
    }

}
 