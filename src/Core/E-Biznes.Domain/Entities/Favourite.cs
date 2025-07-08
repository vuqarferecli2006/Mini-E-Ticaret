using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace E_Biznes.Domain.Entities;

public class Favourite:BaseEntity
{
    public string UserId { get; set; }=null!;
    public AppUser? User { get; set; } 

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
}
