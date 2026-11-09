namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller supporting supply document för en certification assertion.
    /// </summary>
    public sealed class SetSupplyCertificationDocumentRequest
    {
        /// <summary>
        /// Identifierar supporting supply document, eller null för att ta bort kopplingen.
        /// </summary>
        public Guid? SupplyDocumentId { get; init; }

        /// <summary>
        /// Concurrency-token från senast lästa certification.
        /// </summary>
        public uint RowVersion { get; init; }
    }
}
