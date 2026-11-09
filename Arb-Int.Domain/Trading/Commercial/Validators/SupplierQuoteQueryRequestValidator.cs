using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar sökningar efter SupplierQuote.
/// </summary>
public sealed class SupplierQuoteQueryRequestValidator : AbstractValidator<SupplierQuoteQueryRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för SupplierQuote-filtrering och paginering.
    /// </summary>
    public SupplierQuoteQueryRequestValidator()
    {
        RuleFor(x => x.Search).MaximumLength(255);
        RuleFor(x => x.SupplierBusinessRelationshipId).NotEqual(Guid.Empty).When(x => x.SupplierBusinessRelationshipId.HasValue);
        RuleFor(x => x.SupplierPartyId).NotEqual(Guid.Empty).When(x => x.SupplierPartyId.HasValue);
        RuleFor(x => x.BuyerRfqId).NotEqual(Guid.Empty).When(x => x.BuyerRfqId.HasValue);
        RuleFor(x => x.BuyerRfqRevisionId).NotEqual(Guid.Empty).When(x => x.BuyerRfqRevisionId.HasValue);
        RuleFor(x => x.Status).IsInEnum().When(x => x.Status.HasValue);
        RuleFor(x => x.OwnerUserId).NotEqual(Guid.Empty).When(x => x.OwnerUserId.HasValue);
        RuleFor(x => x.SortBy).IsInEnum();
        RuleFor(x => x.SortDirection).IsInEnum();
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 200);
    }
}
