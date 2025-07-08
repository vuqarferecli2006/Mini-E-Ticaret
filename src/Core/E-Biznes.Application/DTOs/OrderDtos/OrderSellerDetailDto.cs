namespace E_Biznes.Application.DTOs.OrderDtos;

public record OrderSellerDetailDto
{
    public string BuyerName { get; set; } = null!;
    public string BuyerEmail { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal Total { get; set; }
}
