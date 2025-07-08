namespace E_Biznes.Application.DTOs.OrderDtos;

public record OrderCreateDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
