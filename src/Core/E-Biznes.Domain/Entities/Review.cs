namespace E_Biznes.Domain.Entities;

public class Review : BaseEntity
{
    public string Content { get; set; } = string.Empty!;

    public int Rating { get; set; }

    public Guid ProductId { get; set; }
    public Product? Product { get; set; } 

    public Guid UserId { get; set; }
    public AppUser? User { get; set; }
}
