namespace E_Biznes.Application.DTOs.OrderDtos;

public class OrderEmailDetailsDto
{
    public string FullName { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}
