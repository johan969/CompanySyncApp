namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller redigerbar metadata för buyer-proposal-roten.
/// </summary>
public sealed class UpdateBuyerProposalRequest
{
    /// <summary>
    /// Anger proposalets namn.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Beskriver proposalet.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Identifierar eventuell intern ägare.
    /// </summary>
    public Guid? OwnerUserId { get; init; }

    /// <summary>
    /// Innehåller interna anteckningar.
    /// </summary>
    public string? InternalNotes { get; init; }

    /// <summary>
    /// Concurrency-token från senast lästa proposal.
    /// </summary>
    public uint RowVersion { get; init; }
}