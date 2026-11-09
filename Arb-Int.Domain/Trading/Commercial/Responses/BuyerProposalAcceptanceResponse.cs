namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller en registrerad buyer acceptance och dess interna approval-status.
/// </summary>
public sealed class BuyerProposalAcceptanceResponse
{
    /// <summary>
    /// Identifierar acceptancen.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Identifierar proposal-revisionen.
    /// </summary>
    public Guid BuyerProposalRevisionId { get; init; }

    /// <summary>
    /// Identifierar proposal-optionen.
    /// </summary>
    public Guid BuyerProposalOptionId { get; init; }

    /// <summary>
    /// Anger acceptance-workflow-status.
    /// </summary>
    public BuyerProposalAcceptanceStatus Status { get; init; }

    /// <summary>
    /// Anger eventuell extern buyer-referens.
    /// </summary>
    public string? BuyerAcceptanceReference { get; init; }

    /// <summary>
    /// Anger när köparen faktiskt accepterade.
    /// </summary>
    public DateTimeOffset AcceptedAtUtc { get; init; }

    /// <summary>
    /// Identifierar eventuell användare som registrerade acceptansen.
    /// </summary>
    public Guid? RecordedByUserId { get; init; }

    /// <summary>
    /// Anger om denna acceptance i sig accepterar mindre än hela optionen.
    /// </summary>
    public bool IsPartialAcceptance { get; init; }

    /// <summary>
    /// Anger när acceptansen godkändes internt.
    /// </summary>
    public DateTimeOffset? InternallyApprovedAtUtc { get; init; }

    /// <summary>
    /// Identifierar eventuell användare som godkände acceptansen.
    /// </summary>
    public Guid? InternallyApprovedByUserId { get; init; }

    /// <summary>
    /// Innehåller interna approval-anteckningar.
    /// </summary>
    public string? InternalApprovalNotes { get; init; }

    /// <summary>
    /// Innehåller de quantities som ingår i just denna acceptance.
    /// </summary>
    public required IReadOnlyList<BuyerProposalAcceptanceLineResponse> Lines { get; init; }

    /// <summary>
    /// Concurrency-token från databasen.
    /// </summary>
    public uint RowVersion { get; init; }
}