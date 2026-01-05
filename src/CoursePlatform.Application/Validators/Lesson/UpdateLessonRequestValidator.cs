using CoursePlatform.Application.DTOs.Lesson;
using FluentValidation;

namespace CoursePlatform.Application.Validators.Lesson;

public class UpdateLessonRequestValidator : AbstractValidator<UpdateLessonRequest>
{
    public UpdateLessonRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("El título de la lección es obligatorio.")
            .MaximumLength(200).WithMessage("El título de la lección no puede exceder los 200 caracteres.");

        RuleFor(x => x.Order)
            .GreaterThan(0).WithMessage("El orden debe ser mayor a 0.");
    }
}