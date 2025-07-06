namespace E_Biznes.Application.DTOs.OrderDtos;

public class CreateOrderDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
