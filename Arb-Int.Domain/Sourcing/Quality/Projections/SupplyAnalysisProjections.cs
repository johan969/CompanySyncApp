using System.Linq.Expressions;

namespace Arb_Int.Domain.Sourcing.Projections
{
    /// <summary>
    /// Innehåller återanvändbara EF-projektioner för supply analyses.
    /// </summary>
    public static class SupplyAnalysisProjections
    {
        /// <summary>
        /// Projekterar en analysis till listmodellen.
        /// </summary>
        public static Expression<Func<SupplyAnalysis, SupplyAnalysisListResponse>> List => analysis => new SupplyAnalysisListResponse
        {
            Id = analysis.Id,
            SupplierMaterialOfferingId = analysis.SupplierMaterialOfferingId,
            InventoryLotId = analysis.InventoryLotId,
            SupplyDocumentId = analysis.SupplyDocumentId,
            LaboratoryBusinessRelationshipId = analysis.LaboratoryBusinessRelationshipId,
            Status = analysis.Status,
            CreateDateUtc = analysis.CreateDateUtc,
            RowVersion = analysis.RowVersion
        };

        /// <summary>
        /// Projekterar en analysis med dess resultat.
        /// </summary>
        public static Expression<Func<SupplyAnalysis, SupplyAnalysisResponse>> Details => analysis => new SupplyAnalysisResponse
        {
            Id = analysis.Id,
            SupplierMaterialOfferingId = analysis.SupplierMaterialOfferingId,
            InventoryLotId = analysis.InventoryLotId,
            SupplyDocumentId = analysis.SupplyDocumentId,
            LaboratoryBusinessRelationshipId = analysis.LaboratoryBusinessRelationshipId,
            Status = analysis.Status,
            CreateDateUtc = analysis.CreateDateUtc,
            Results = analysis.Results
                .OrderBy(x => x.ConstituentId)
                .ThenBy(x => x.CharacteristicId)
                .ThenBy(x => x.AnalyteName)
                .ThenBy(x => x.Id)
                .Select(x => new SupplyAnalysisResultResponse
                {
                    Id = x.Id,
                    SupplyAnalysisId = x.SupplyAnalysisId,
                    ConstituentId = x.ConstituentId,
                    CharacteristicId = x.CharacteristicId,
                    AnalyteName = x.AnalyteName,
                    Qualifier = x.Qualifier,
                    NumericValue = x.NumericValue,
                    RangeMaximumValue = x.RangeMaximumValue,
                    TextValue = x.TextValue,
                    UnitOfMeasureId = x.UnitOfMeasureId,
                    Basis = x.Basis,
                    DetectionLimit = x.DetectionLimit,
                    AnalysisMethod = x.AnalysisMethod,
                    VerificationState = x.VerificationState,
                    Notes = x.Notes,
                    RowVersion = x.RowVersion
                })
                .ToList(),
            RowVersion = analysis.RowVersion
        };
    }
}
