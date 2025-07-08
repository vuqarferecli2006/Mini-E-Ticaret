namespace E_Biznes.Application.DTOs.OrderDtos;

public record OrderDiscountDto
{
    public Guid OrderId { get; set; }
    public decimal OriginalTotalPrice { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal DiscountedTotalPrice { get; set; }
}
