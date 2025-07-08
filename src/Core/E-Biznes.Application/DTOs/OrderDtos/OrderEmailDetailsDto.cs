namespace E_Biznes.Application.DTOs.OrderDtos;

public record OrderEmailDetailsDto
{
    public string FullName { get; set; } =null!;
    public string ProductName { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}
