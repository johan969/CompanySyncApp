namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller den kvantitet från en proposal-line som köparen accepterat.
/// </summary>
public sealed class RecordBuyerProposalAcceptanceLineRequest
{
    /// <summary>
    /// Identifierar proposal-raden som köparen accepterar helt eller delvis.
    /// </summary>
    public Guid BuyerProposalLineId { get; init; }

    /// <summary>
    /// Anger accepterad kvantitet i proposal-radens befintliga QuantityUnit.
    /// </summary>
    public decimal AcceptedQuantity { get; init; }

    /// <summary>
    /// Innehåller buyer-noteringar kopplade till acceptansen av raden.
    /// </summary>
    public string? BuyerNotes { get; init; }

    /// <summary>
    /// Innehåller interna anteckningar om den accepterade raden.
    /// </summary>
    public string? InternalNotes { get; init; }
}