using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar sökningar efter BuyerProposal.
/// </summary>
public sealed class BuyerProposalQueryRequestValidator : AbstractValidator<BuyerProposalQueryRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för BuyerProposal-filtrering och paginering.
    /// </summary>
    public BuyerProposalQueryRequestValidator()
    {
        RuleFor(x => x.Search).MaximumLength(255);
        RuleFor(x => x.BuyerBusinessRelationshipId).NotEqual(Guid.Empty).When(x => x.BuyerBusinessRelationshipId.HasValue);
        RuleFor(x => x.BuyerPartyId).NotEqual(Guid.Empty).When(x => x.BuyerPartyId.HasValue);
        RuleFor(x => x.BuyerRfqId).NotEqual(Guid.Empty).When(x => x.BuyerRfqId.HasValue);
        RuleFor(x => x.Status).IsInEnum().When(x => x.Status.HasValue);
        RuleFor(x => x.OwnerUserId).NotEqual(Guid.Empty).When(x => x.OwnerUserId.HasValue);
        RuleFor(x => x.SortBy).IsInEnum();
        RuleFor(x => x.SortDirection).IsInEnum();
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 200);
    }
}
