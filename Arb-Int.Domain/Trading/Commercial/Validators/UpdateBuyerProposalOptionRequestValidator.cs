using System.Text.Json;
using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar ändringar på Draft buyer-proposal-options.
/// </summary>
public sealed class UpdateBuyerProposalOptionRequestValidator : AbstractValidator<UpdateBuyerProposalOptionRequest>
{
    /// <summary>
    /// Konfigurerar option- och concurrency-regler.
    /// </summary>
    public UpdateBuyerProposalOptionRequestValidator()
    {
        RuleFor(x => x.OptionNumber).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.BuyerFacingSummary).MaximumLength(8000);
        RuleFor(x => x.RowVersion).NotEqual(0u);

        RuleFor(x => x.InternalMarginSnapshotJson)
            .Must(BeValidJson)
            .When(x => !string.IsNullOrWhiteSpace(x.InternalMarginSnapshotJson))
            .WithMessage("InternalMarginSnapshotJson must contain valid JSON.");
    }

    /// <summary>
    /// Kontrollerar att text innehåller giltig JSON.
    /// </summary>
    private static bool BeValidJson(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return true;
        }

        try
        {
            using var document = JsonDocument.Parse(json);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}