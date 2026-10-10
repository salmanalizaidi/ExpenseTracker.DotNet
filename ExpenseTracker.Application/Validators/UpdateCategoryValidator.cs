using ExpenseTracker.Application.DTOs;
using FluentValidation;

namespace ExpenseTracker.Application.Validators;

public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryDto>
{
    public UpdateCategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(100).WithMessage("Category name cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(300).WithMessage("Description cannot exceed 300 characters.")
            .When(x => x.Description is not null);

        RuleFor(x => x.Icon)
            .MaximumLength(50).WithMessage("Icon identifier cannot exceed 50 characters.")
            .When(x => x.Icon is not null);
    }
}
