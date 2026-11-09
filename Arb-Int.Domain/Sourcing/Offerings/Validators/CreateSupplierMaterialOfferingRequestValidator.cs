using FluentValidation;

namespace Arb_Int.Domain.Sourcing;

/// <summary>
/// Validerar grunddata för ett nytt supplier material offering.
/// </summary>
public sealed class CreateSupplierMaterialOfferingRequestValidator : AbstractValidator<CreateSupplierMaterialOfferingRequest>
{
    /// <summary>
    /// Skapar regler för nya offerings.
    /// </summary>
    public CreateSupplierMaterialOfferingRequestValidator()
    {
        RuleFor(x => x.OfferingKey)
            .NotEmpty()
            .MaximumLength(128);

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
                    if (request.FirstObservedUtc.HasValue
                        && request.LastObservedUtc.HasValue
                        && request.LastObservedUtc.Value < request.FirstObservedUtc.Value)
                    {
                        context.AddFailure(
                            nameof(request.LastObservedUtc),
                            "LastObservedUtc cannot be earlier than FirstObservedUtc.");
                    }

                    var resolvedFirstObservedUtc =
                        request.FirstObservedUtc
                        ?? request.LastObservedUtc;

                    if (resolvedFirstObservedUtc.HasValue
                        && request.LastVerifiedUtc.HasValue
                        && request.LastVerifiedUtc.Value < resolvedFirstObservedUtc.Value)
                    {
                        context.AddFailure(
                            nameof(request.LastVerifiedUtc),
                            "LastVerifiedUtc cannot be earlier than FirstObservedUtc.");
                    }
                });
    }
}
