namespace E_Biznes.Application.DTOs.ProducDtos;

public record ProductSimpleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
}
