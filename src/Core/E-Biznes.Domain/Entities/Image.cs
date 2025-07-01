namespace E_Biznes.Domain.Entities;

public class Image:BaseEntity
{
    public string Image_Url { get; set; }=string.Empty!;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public bool IsMain { get; set; }
}

