using System.Linq.Expressions;
namespace Arb_Int.Domain.Trading.Projections;

/// <summary>
/// Innehåller EF Core-projections för RFQ-rader och materialalternativ.
/// </summary>
public static class BuyerRfqLineProjections
{
    /// <summary>
    /// Projekterar en RFQ-rad med materialalternativ till response-objektet.
    /// </summary>
    public static Expression<Func<BuyerRfqLine, BuyerRfqLineResponse>> Details => line => new BuyerRfqLineResponse
    {
        Id = line.Id,
        BuyerRfqRevisionId = line.BuyerRfqRevisionId,
        BuyerRfqId = line.BuyerRfqRevision.BuyerRfqId,
        RevisionStatus = line.BuyerRfqRevision.Status,
        BuyerDemandScopeId = line.BuyerDemandScopeId,
        LineNumber = line.LineNumber,
        Name = line.Name,
        Description = line.Description,
        BuyerLineReference = line.BuyerLineReference,
        BuyerOriginalText = line.BuyerOriginalText,
        MaterialId = line.MaterialId,
        MaterialTypeId = line.MaterialTypeId,
        IntendedUseCharacteristicId = line.IntendedUseCharacteristicId,
        RequiredQuantity = line.RequiredQuantity,
        QuantityUnitId = line.QuantityUnitId,
        LowerQuantityTolerancePercent = line.LowerQuantityTolerancePercent,
        UpperQuantityTolerancePercent = line.UpperQuantityTolerancePercent,
        AllowPartialFulfillment = line.AllowPartialFulfillment,
        AllowMultipleSuppliers = line.AllowMultipleSuppliers,
        AllowMultipleOrigins = line.AllowMultipleOrigins,
        AllowMultipleLots = line.AllowMultipleLots,
        RequireSingleSupplier = line.RequireSingleSupplier,
        RequireSingleOrigin = line.RequireSingleOrigin,
        RequireSingleLot = line.RequireSingleLot,
        RequireUniformCertification = line.RequireUniformCertification,
        RequireUniformSpecification = line.RequireUniformSpecification,
        MaximumSupplierCount = line.MaximumSupplierCount,
        InternalNotes = line.InternalNotes,
        RowVersion = line.RowVersion,
        RequirementCount = line.Requirements.Count,
        DeliveryAllocationCount = line.DeliveryAllocations.Count,
        MaterialOptions = line.MaterialOptions
            .OrderBy(x => x.OptionKind)
            .ThenBy(x => x.SortOrder)
            .ThenBy(x => x.Id)
            .Select(x => new BuyerRfqLineMaterialOptionResponse
            {
                Id = x.Id,
                BuyerRfqLineId = x.BuyerRfqLineId,
                OptionKind = x.OptionKind,
                MaterialId = x.MaterialId,
                MaterialTypeId = x.MaterialTypeId,
                BiologicalSourceId = x.BiologicalSourceId,
                BiologicalSourceClaimId = x.BiologicalSourceClaimId,
                BuyerText = x.BuyerText,
                AlternativeGroupKey = x.AlternativeGroupKey,
                MinimumMappingConfidence = x.MinimumMappingConfidence,
                SortOrder = x.SortOrder,
                RowVersion = x.RowVersion
            })
            .ToList()
    };
}