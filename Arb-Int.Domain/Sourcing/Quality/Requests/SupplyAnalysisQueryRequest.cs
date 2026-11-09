using Arb_Int.Domain.Services.Common;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Anger filtrering, sortering och paginering för supply analyses.
    /// </summary>
    public sealed class SupplyAnalysisQueryRequest
    {
        /// <summary>
        /// Begränsar resultatet till ett supplier material offering.
        /// </summary>
        public Guid? SupplierMaterialOfferingId { get; init; }

        /// <summary>
        /// Begränsar resultatet till en inventory lot.
        /// </summary>
        public Guid? InventoryLotId { get; init; }

        /// <summary>
        /// Begränsar resultatet till ett supporting supply document.
        /// </summary>
        public Guid? SupplyDocumentId { get; init; }

        /// <summary>
        /// Begränsar resultatet till ett laboratory business relationship.
        /// </summary>
        public Guid? LaboratoryBusinessRelationshipId { get; init; }

        /// <summary>
        /// Begränsar resultatet till en analysis status.
        /// </summary>
        public AnalysisStatus? Status { get; init; }


        /// <summary>
        /// Anger sorteringsfält.
        /// </summary>
        public SupplyAnalysisSortField SortBy { get; init; } = SupplyAnalysisSortField.CreateDateUtc;

        /// <summary>
        /// Anger sorteringsriktning.
        /// </summary>
        public SortDirection SortDirection { get; init; } = SortDirection.Descending;

        /// <summary>
        /// Sidnummer med start på 1.
        /// </summary>
        public int Page { get; init; } = 1;

        /// <summary>
        /// Maximalt antal poster per sida.
        /// </summary>
        public int PageSize { get; init; } = 50;
    }

    /// <summary>
    /// Anger tillgängliga sorteringsfält för supply analyses.
    /// </summary>
    public enum SupplyAnalysisSortField
    {
        /// <summary>
        /// Sorterar på skapandetid.
        /// </summary>
        CreateDateUtc = 0,

        /// <summary>
        /// Sorterar på status.
        /// </summary>
        Status = 1,

    }
}
