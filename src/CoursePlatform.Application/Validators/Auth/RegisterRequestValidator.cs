using CoursePlatform.Application.DTOs.Auth;
using FluentValidation;

namespace CoursePlatform.Application.Validators.Auth;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es obligatorio.")
            .EmailAddress().WithMessage("El formato del email no es válido.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria.")
            .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("El apellido es obligatorio.")
            .MaximumLength(50).WithMessage("El apellido no puede exceder los 50 caracteres.");
    }
}