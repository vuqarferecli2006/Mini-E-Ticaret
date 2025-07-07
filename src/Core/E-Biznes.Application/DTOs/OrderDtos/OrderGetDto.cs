using E_Biznes.Domain.Enum;

namespace E_Biznes.Application.DTOs.OrderDtos;

public record OrderGetDto
{
    public Guid Id { get; set; }
    public decimal TotalPrice { get; set; }
    public string? UserName { get; set; }
    public List<OrderProductDto> OrderProducts { get; set; } = new();
}
