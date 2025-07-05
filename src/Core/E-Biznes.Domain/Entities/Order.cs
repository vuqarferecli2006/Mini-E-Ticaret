using E_Biznes.Domain.Enum;

namespace E_Biznes.Domain.Entities;

public class Order:BaseEntity
{

    public string? UserId { get; set; }

    public AppUser? User { get; set; } 

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public OrderStatus Status { get; set; }= OrderStatus.Pending;

    public decimal TotalPrice { get; set; }

    public ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

}
