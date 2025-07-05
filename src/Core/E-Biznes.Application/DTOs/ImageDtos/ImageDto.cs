namespace E_Biznes.Application.DTOs.ImageDtos;

public class ImageDto
{
    public Guid Id { get; set; }
    public string Image_Url { get; set; } = string.Empty;
    public bool IsMain { get; set; }
}
