namespace E_Biznes.Application.DTOs.OrderDtos;

public record CreateOrderDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
