namespace E_Biznes.Application.DTOs.CategoryDtos;

public record CategoryUpdateDto
(
     string Name ,
     string? Description ,
     Guid? ParentCategoryId 
);
