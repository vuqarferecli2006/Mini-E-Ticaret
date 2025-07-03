using E_Biznes.Application.DTOs.UserDtos;
using FluentValidation;

namespace E_Biznes.Application.Validations.UserValidations;

public class UserLoginValidtor: AbstractValidator<UserLoginDto>
{
    public UserLoginValidtor()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
    }
}
