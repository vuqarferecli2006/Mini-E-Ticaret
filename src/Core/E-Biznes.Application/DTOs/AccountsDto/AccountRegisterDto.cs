using E_Biznes.Domain.Enum;

namespace E_Biznes.Application.DTOs.AccountsDto;

public record AccountRegisterDto
{
     public string FullName {  get; set; }=string.Empty;
     public string Email { get; set; } = string.Empty;
     public string Password { get; set; } = string.Empty;
     public string Address { get; set; } = string.Empty;
     public int Age {  get; set; }  
     public AccountRole RoleId {  get; set; }
}
