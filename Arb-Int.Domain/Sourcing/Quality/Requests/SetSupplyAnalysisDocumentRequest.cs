namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller supporting supply document för en supply analysis.
    /// </summary>
    public sealed class SetSupplyAnalysisDocumentRequest
    {
        /// <summary>
        /// Identifierar supporting supply document, eller null för att rensa kopplingen.
        /// </summary>
        public Guid? SupplyDocumentId { get; init; }

        /// <summary>
        /// Concurrency-token från senast lästa analysis.
        /// </summary>
        public uint RowVersion { get; init; }
    }
}
