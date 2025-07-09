using E_Biznes.Application.DTOs.UserDtos;
using FluentValidation;

namespace E_Biznes.Application.Validations.UserValidations;

public class UserAddRoleDtoValidator : AbstractValidator<UserAddRoleDto>
{
    public UserAddRoleDtoValidator()
    {
        RuleFor(x => x.UserId)
            .NotEqual(Guid.Empty).WithMessage("User ID must be valid.");

        RuleFor(x => x.RoleId)
            .NotNull().WithMessage("Role list is required.")
            .Must(list => list!.Any()).WithMessage("At least one role must be assigned.")
            .Must(list => list!.Distinct().Count() == list.Count).WithMessage("Duplicate roles are not allowed.");
    }
}
