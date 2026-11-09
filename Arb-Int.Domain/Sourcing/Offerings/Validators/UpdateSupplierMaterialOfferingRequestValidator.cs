using FluentValidation;

namespace Arb_Int.Domain.Sourcing;

/// <summary>
/// Validerar redigerbar metadata för ett supplier material offering.
/// </summary>
public sealed class UpdateSupplierMaterialOfferingRequestValidator : AbstractValidator<UpdateSupplierMaterialOfferingRequest>
{
    /// <summary>
    /// Skapar regler för offering-uppdateringar.
    /// </summary>
    public UpdateSupplierMaterialOfferingRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(512);

        RuleFor(x => x.Description)
            .NotNull();

        RuleFor(x => x.SupplierSku)
            .MaximumLength(255);

        RuleFor(x => x.DeclaredFormat)
            .MaximumLength(512);

        RuleFor(x => x)
            .Custom(
                (request, context) =>
                {
                    if (request.LastObservedUtc < request.FirstObservedUtc)
                    {
                        context.AddFailure(
                            nameof(request.LastObservedUtc),
                            "LastObservedUtc cannot be earlier than FirstObservedUtc.");
                    }

                    if (request.LastVerifiedUtc.HasValue
                        && request.LastVerifiedUtc.Value < request.FirstObservedUtc)
                    {
                        context.AddFailure(
                            nameof(request.LastVerifiedUtc),
                            "LastVerifiedUtc cannot be earlier than FirstObservedUtc.");
                    }
                });
    }
}
