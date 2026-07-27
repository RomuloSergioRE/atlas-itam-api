using FluentValidation;

namespace Atlas.Itam.Application.Commands.Assets.UpdateAsset;

public sealed class UpdateAssetCommandValidator : AbstractValidator<UpdateAssetCommand>
{
    public UpdateAssetCommandValidator()
    {
        RuleFor(x => x.AssetId)
            .NotEmpty().WithMessage("Asset ID is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters");

        RuleFor(x => x.PatrimonyNumber)
            .NotEmpty().WithMessage("Patrimony number is required")
            .MaximumLength(50).WithMessage("Patrimony number must not exceed 50 characters");

        RuleFor(x => x.SerialNumber)
            .NotEmpty().WithMessage("Serial number is required")
            .MaximumLength(100).WithMessage("Serial number must not exceed 100 characters");

        RuleFor(x => x.AcquisitionDate)
            .NotEmpty().WithMessage("Acquisition date is required");

        RuleFor(x => x.AcquisitionValue)
            .GreaterThanOrEqualTo(0).WithMessage("Acquisition value must be non-negative");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category is required");

        RuleFor(x => x.LocationId)
            .NotEmpty().WithMessage("Location is required");
    }
}
