namespace E_Biznes.Application.DTOs.FavouriteDto;

public class FavouriteDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public decimal DiscountPercent { get; set; }
    public decimal DiscountedPrice { get; set; }
    public string? MainImageUrl { get; set; }
}
