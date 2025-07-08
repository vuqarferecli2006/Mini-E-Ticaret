using E_Biznes.Application.DTOs.ProducDtos;

namespace E_Biznes.Application.DTOs.UserDtos;

public record UserProfileDto
{
    public string Id { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Address { get; set; } = null!;
    public int Age { get; set; }
    public string Role { get; set; } = null!;
    public List<ProductSimpleDto>? ProductsForSale { get; set; }
    public List<ProductSimpleDto>? PurchasedProducts { get; set; }
}
