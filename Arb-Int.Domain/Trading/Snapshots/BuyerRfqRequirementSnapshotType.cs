namespace Arb_Int.Domain.Trading.Snapshots;

/// <summary>
/// Anger den konkreta requirement-typen i snapshot-formatet.
/// </summary>
public enum BuyerRfqRequirementSnapshotType
{
    Material,
    BiologicalSource,
    Origin,
    Certification,
    Document,
    Constituent,
    Characteristic,
    Process,
    Commercial,
    Logistics
}