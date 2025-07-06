namespace E_Biznes.Application.DTOs.OrderDtos;

public class OrderProductDto
{
    public Guid ProductId { get; set; }
    
    public string ProductName { get; set; } = string.Empty;

    public int Quantity { get; set; }
    
    public decimal UnitPrice { get; set; }
}
