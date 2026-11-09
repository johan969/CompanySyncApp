using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar registrering av buyer acceptance.
/// </summary>
public sealed class RecordBuyerProposalAcceptanceRequestValidator : AbstractValidator<RecordBuyerProposalAcceptanceRequest>
{
    /// <summary>
    /// Konfigurerar acceptance-, line- och concurrency-regler.
    /// </summary>
    public RecordBuyerProposalAcceptanceRequestValidator()
    {
        RuleFor(x => x.BuyerAcceptanceReference).MaximumLength(255);
        RuleFor(x => x.RecordedByUserId).NotEqual(Guid.Empty).When(x => x.RecordedByUserId.HasValue);
        RuleFor(x => x.Lines).NotEmpty();
        RuleForEach(x => x.Lines).SetValidator(new RecordBuyerProposalAcceptanceLineRequestValidator());
        RuleFor(x => x.BuyerProposalRowVersion).NotEqual(0u);
        RuleFor(x => x.BuyerProposalRevisionRowVersion).NotEqual(0u);
        RuleFor(x => x.BuyerProposalOptionRowVersion).NotEqual(0u);

        RuleFor(x => x.Lines)
            .Must(HaveUniqueLines)
            .WithMessage("BuyerProposalLineId must be unique within one BuyerProposalAcceptance.");
    }

    /// <summary>
    /// Kontrollerar att samma proposal-line inte förekommer flera gånger i samma acceptance.
    /// </summary>
    private static bool HaveUniqueLines(
        IReadOnlyList<RecordBuyerProposalAcceptanceLineRequest> lines)
    {
        return lines
            .Select(x => x.BuyerProposalLineId)
            .Distinct()
            .Count() == lines.Count;
    }
}