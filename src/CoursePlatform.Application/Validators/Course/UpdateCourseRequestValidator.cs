using CoursePlatform.Application.DTOs.Course;
using FluentValidation;

namespace CoursePlatform.Application.Validators.Course;

public class UpdateCourseRequestValidator : AbstractValidator<UpdateCourseRequest>
{
    public UpdateCourseRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("El título del curso es obligatorio.")
            .MaximumLength(200).WithMessage("El título del curso no puede exceder los 200 caracteres.");
    }
}