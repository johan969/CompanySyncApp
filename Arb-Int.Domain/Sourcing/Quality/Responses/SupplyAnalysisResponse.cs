namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller en supply analysis med dess resultat.
    /// </summary>
    public sealed class SupplyAnalysisResponse
    {
        /// <summary>
        /// Identifierar analysen.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Identifierar supplier material offering när analysen gäller offeringet.
        /// </summary>
        public Guid? SupplierMaterialOfferingId { get; init; }

        /// <summary>
        /// Identifierar inventory lot när analysen gäller en lot.
        /// </summary>
        public Guid? InventoryLotId { get; init; }

        /// <summary>
        /// Identifierar supporting supply document när ett sådant finns.
        /// </summary>
        public Guid? SupplyDocumentId { get; init; }

        /// <summary>
        /// Identifierar laboratory business relationship när det är känt.
        /// </summary>
        public Guid? LaboratoryBusinessRelationshipId { get; init; }

        /// <summary>
        /// Anger analysens status.
        /// </summary>
        public AnalysisStatus Status { get; init; }


        /// <summary>
        /// Anger när analysen registrerades i systemet.
        /// </summary>
        public DateTimeOffset? CreateDateUtc { get; init; }

        /// <summary>
        /// Innehåller analysens mät- och textresultat.
        /// </summary>
        public required IReadOnlyList<SupplyAnalysisResultResponse> Results { get; init; }

        /// <summary>
        /// Concurrency-token för analysis-aggregatet.
        /// </summary>
        public uint RowVersion { get; init; }
    }
}
