using E_Biznes.Domain.Enum;

namespace E_Biznes.Domain.Entities;

public class Product: BaseEntity
{
    public string Name { get; set; } =string.Empty!;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public ProductCondition Condition { get; set; }

    public string? UserId { get; set; }
    public AppUser User { get; set; } = null!;

    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public ICollection<Favourite> Favourites { get; set; }=new List<Favourite>();
    public ICollection<Image> ProductImages { get; set; }= new List<Image>();
    public ICollection<OrderProduct> OrderProducts { get; set; }= new List<OrderProduct>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();

}
