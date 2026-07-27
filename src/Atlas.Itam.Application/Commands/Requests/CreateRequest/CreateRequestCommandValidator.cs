using FluentValidation;

namespace Atlas.Itam.Application.Commands.Requests.CreateRequest;

public sealed class CreateRequestCommandValidator : AbstractValidator<CreateRequestCommand>
{
    public CreateRequestCommandValidator()
    {
        RuleFor(x => x.Justification)
            .NotEmpty().WithMessage("Justification is required")
            .MaximumLength(1000).WithMessage("Justification must not exceed 1000 characters");

        RuleFor(x => x.AssetId)
            .NotEmpty().WithMessage("Asset is required");

        RuleFor(x => x.RequestedById)
            .NotEmpty().WithMessage("Requested by user is required");
    }
}
