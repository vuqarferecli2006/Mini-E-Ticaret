namespace E_Biznes.Application.DTOs.OrderDtos;

public record OrderDto
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
}
