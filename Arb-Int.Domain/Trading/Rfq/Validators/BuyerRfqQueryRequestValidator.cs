using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar sökningar efter BuyerRfq.
/// </summary>
public sealed class BuyerRfqQueryRequestValidator : AbstractValidator<BuyerRfqQueryRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för RFQ-filtrering och paginering.
    /// </summary>
    public BuyerRfqQueryRequestValidator()
    {
        RuleFor(x => x.Search).MaximumLength(255);
        RuleFor(x => x.Status).IsInEnum().When(x => x.Status.HasValue);
        RuleFor(x => x.Priority).IsInEnum().When(x => x.Priority.HasValue);
        RuleFor(x => x.Visibility).IsInEnum().When(x => x.Visibility.HasValue);
        RuleFor(x => x.OwnerUserId).NotEqual(Guid.Empty).When(x => x.OwnerUserId.HasValue);
        RuleFor(x => x.BuyerPartyId).NotEqual(Guid.Empty).When(x => x.BuyerPartyId.HasValue);
        RuleFor(x => x.SortBy).IsInEnum();
        RuleFor(x => x.SortDirection).IsInEnum();
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 200);
    }
}