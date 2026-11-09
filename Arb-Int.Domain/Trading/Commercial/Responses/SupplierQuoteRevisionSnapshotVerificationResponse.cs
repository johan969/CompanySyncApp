namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller resultatet från verifiering av ett supplier-quote-snapshot.
/// </summary>
public sealed class SupplierQuoteRevisionSnapshotVerificationResponse
{
    /// <summary>
    /// Identifierar revisionen.
    /// </summary>
    public Guid SupplierQuoteRevisionId { get; init; }

    /// <summary>
    /// Anger om sparad JSON och sparad hash fortfarande stämmer överens.
    /// </summary>
    public bool IsValid { get; init; }

    /// <summary>
    /// Innehåller sparad hash.
    /// </summary>
    public required string StoredHash { get; init; }

    /// <summary>
    /// Innehåller hash beräknad från sparad jsonb.
    /// </summary>
    public required string ComputedHash { get; init; }
}