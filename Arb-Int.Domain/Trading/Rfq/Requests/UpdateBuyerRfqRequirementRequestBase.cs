namespace Arb_Int.Domain.Trading;

/// <summary>
/// Utökar gemensamma requirement-värden med status och optimistic concurrency för uppdateringar.
/// </summary>
public abstract class UpdateBuyerRfqRequirementRequestBase : BuyerRfqRequirementRequestBase
{
    /// <summary>
    /// Anger om kravet ska vara aktivt vid utvärdering.
    /// </summary>
    public bool IsActive { get; init; } = true;

    /// <summary>
    /// Concurrency-token från senast lästa version.
    /// </summary>
    public uint RowVersion { get; init; }
}