using FluentValidation;
using TaskManagement.Application.DTOs;

namespace TaskManagement.Application.Validators;

public class UpdateTaskRequestValidator : AbstractValidator<UpdateTaskRequest>
{
    public UpdateTaskRequestValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.")
            .Must(t => !string.IsNullOrWhiteSpace(t!)).WithMessage("Title must not be whitespace.")
            .When(x => x.Title != null);

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.")
            .When(x => x.Description != null);

        RuleFor(x => x.DueDate)
            .Must(date => date!.Value.Date >= DateTime.UtcNow.Date).WithMessage("Due date cannot be in the past.")
            .When(x => x.DueDate != null);

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid status value.")
            .When(x => x.Status != null);
    }
}
