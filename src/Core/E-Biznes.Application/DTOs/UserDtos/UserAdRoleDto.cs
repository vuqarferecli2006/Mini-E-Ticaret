namespace E_Biznes.Application.DTOs.UserDtos;

public record UserAddRoleDto
(
     Guid UserId ,

     List<Guid>? RoleId 
);
