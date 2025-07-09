using E_Biznes.Domain.Enum;

namespace E_Biznes.Application.DTOs.OrderDtos;

public record OrderGetDto
{
    public Guid Id { get; set; }
    public decimal TotalPrice { get; set; }
    public string? UserFullName { get; set; }
    public string Status { get; set; }=null!;
    public List<OrderProductDto> OrderProducts { get; set; } = new();
}
