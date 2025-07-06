using E_Biznes.Domain.Enum;

namespace E_Biznes.Application.DTOs.OrderDtos;

public class OrderGetDto
{
    public Guid Id { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public decimal TotalPrice { get; set; }
    public List<OrderProductDto> OrderProducts { get; set; } = new();
}
