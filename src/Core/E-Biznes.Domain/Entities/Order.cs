namespace E_Biznes.Domain.Entities;

public class Order:BaseEntity
{

    public Guid UserId { get; set; }
    public AppUser User { get; set; } = null!;

    public ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
}
