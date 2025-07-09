using E_Biznes.Application.DTOs.AccountsDto;
using FluentValidation;

public class AccountRegisterDtoValidator : AbstractValidator<AccountRegisterDto>
{
    public AccountRegisterDtoValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email is invalid.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required.");

        RuleFor(x => x.Age)
            .InclusiveBetween(13, 100).WithMessage("Age must be between 13 and 100.");

        RuleFor(x => x.RoleId)
            .IsInEnum().WithMessage("Invalid role selected.");
    }
}
