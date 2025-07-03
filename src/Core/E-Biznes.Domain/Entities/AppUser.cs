using Microsoft.AspNetCore.Identity;

namespace E_Biznes.Domain.Entities;

public class AppUser:IdentityUser
{
    public string FullName { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiryTime { get; set; }

    public int Age { get; set; }

    public ICollection<Favourite> Favourites { get; set; }= new List<Favourite>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Product> Products { get; set; }= new List<Product>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
