using E_Biznes.Domain.Enum;

namespace E_Biznes.Application.DTOs.ProducDtos;

public record ProductFilterParams
{
    public Guid? CategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? Search { get; set; }
    public Rating? MinRating { get; set; }
    public Rating? MaxRating { get; set; }
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; }
}
