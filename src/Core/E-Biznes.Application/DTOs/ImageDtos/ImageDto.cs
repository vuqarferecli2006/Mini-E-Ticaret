namespace E_Biznes.Application.DTOs.ImageDtos;

public record ImageDto
{
    public Guid Id { get; set; }
    public string Image_Url { get; set; } = null!;
    public bool IsMain { get; set; }
}
