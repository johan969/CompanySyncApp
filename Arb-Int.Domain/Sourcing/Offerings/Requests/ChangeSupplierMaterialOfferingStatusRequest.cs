namespace Arb_Int.Domain.Sourcing;

/// <summary>
/// Innehåller den nya kommersiella observationsstatusen för ett supplier material offering.
/// </summary>
public sealed class ChangeSupplierMaterialOfferingStatusRequest
{
    /// <summary>
    /// Anger den nya offering-statusen.
    /// </summary>
    public SupplierOfferingStatus Status { get; init; }

    /// <summary>
    /// Concurrency-token från senast lästa offering.
    /// </summary>
    public uint RowVersion { get; init; }
}
