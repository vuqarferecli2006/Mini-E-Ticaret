using E_Biznes.Application.DTOs.AccountsDto;
using FluentValidation;

namespace E_Biznes.Application.Validations.AccountValidations;

public class AccountGetDtoValidator: AbstractValidator<AccountGetDto>
{
    public AccountGetDtoValidator()
    {
        RuleFor(a=>a.Id)
            .NotEmpty().WithMessage("Id cannot be empty.")
            .NotNull().WithMessage("Id cannot be null.")
            .Must(id => Guid.TryParse(id, out _)).WithMessage("Id must be a valid GUID.");
    }
}
