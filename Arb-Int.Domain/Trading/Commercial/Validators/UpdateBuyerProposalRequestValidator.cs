using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar metadataändringar på buyer-proposal-roten.
/// </summary>
public sealed class UpdateBuyerProposalRequestValidator : AbstractValidator<UpdateBuyerProposalRequest>
{
    /// <summary>
    /// Konfigurerar metadata- och concurrency-regler.
    /// </summary>
    public UpdateBuyerProposalRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.OwnerUserId).NotEqual(Guid.Empty).When(x => x.OwnerUserId.HasValue);
        RuleFor(x => x.InternalNotes).MaximumLength(4096);
        RuleFor(x => x.RowVersion).NotEqual(0u);
    }
}