namespace Arb_Int.Domain.Trading;

/// <summary>
/// Anger den konkreta typen av ett RFQ-krav.
/// </summary>
public enum BuyerRfqRequirementType
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