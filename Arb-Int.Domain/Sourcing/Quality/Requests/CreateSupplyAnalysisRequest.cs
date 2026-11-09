namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller kärnreferenser för att registrera en supply analysis.
    /// </summary>
    public sealed class CreateSupplyAnalysisRequest
    {
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
        /// Identifierar laboratory business relationship när laboratoriet är känt.
        /// </summary>
        public Guid? LaboratoryBusinessRelationshipId { get; init; }
    }
}
