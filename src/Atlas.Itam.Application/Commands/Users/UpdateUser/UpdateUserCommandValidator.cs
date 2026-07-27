using FluentValidation;

namespace Atlas.Itam.Application.Commands.Users.UpdateUser;

public sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required")
            .Must(r => new[] { "Admin", "Manager", "Technician", "Viewer" }.Contains(r))
            .WithMessage("Invalid role");

        RuleFor(x => x.DepartmentId)
            .NotEmpty().WithMessage("Department is required");
    }
}
