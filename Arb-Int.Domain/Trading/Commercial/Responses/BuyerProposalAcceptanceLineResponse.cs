namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller en accepterad quantity från en buyer-proposal-line.
/// </summary>
public sealed class BuyerProposalAcceptanceLineResponse
{
    /// <summary>
    /// Identifierar acceptance-raden.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Identifierar parent-acceptancen.
    /// </summary>
    public Guid BuyerProposalAcceptanceId { get; init; }

    /// <summary>
    /// Identifierar proposal-optionen.
    /// </summary>
    public Guid BuyerProposalOptionId { get; init; }

    /// <summary>
    /// Identifierar proposal-raden.
    /// </summary>
    public Guid BuyerProposalLineId { get; init; }

    /// <summary>
    /// Anger den kvantitet som köparen accepterade i denna acceptance.
    /// </summary>
    public decimal AcceptedQuantity { get; init; }

    /// <summary>
    /// Identifierar quantity unit som kommer från proposal-raden.
    /// </summary>
    public int QuantityUnitId { get; init; }

    /// <summary>
    /// Anger om accepterad quantity skiljer sig från hela proposal-radens quantity.
    /// </summary>
    public bool QuantityChanged { get; init; }

    /// <summary>
    /// Innehåller buyer-noteringar.
    /// </summary>
    public string? BuyerNotes { get; init; }

    /// <summary>
    /// Innehåller interna anteckningar.
    /// </summary>
    public string? InternalNotes { get; init; }

    /// <summary>
    /// Concurrency-token från databasen.
    /// </summary>
    public uint RowVersion { get; init; }
}