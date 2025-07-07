namespace E_Biznes.Application.DTOs.FavouriteDto;

public class FavouriteDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public DateTime AddedDate { get; set; }
}
