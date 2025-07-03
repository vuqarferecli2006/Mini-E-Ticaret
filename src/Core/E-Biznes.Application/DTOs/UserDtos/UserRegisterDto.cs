namespace E_Biznes.Application.DTOs.UserDtos;

public record UserRegisterDto
{
    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; }=string.Empty;

    public string Password { get; set; }=string.Empty;

    public string Address { get; set; } = string.Empty;

    public int Age { get; set; }

}
