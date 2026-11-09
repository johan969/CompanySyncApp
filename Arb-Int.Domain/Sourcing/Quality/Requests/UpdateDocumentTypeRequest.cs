namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller redigerbar metadata för en document type.
    /// </summary>
    public sealed class UpdateDocumentTypeRequest
    {
        /// <summary>
        /// Anger document typens visningsnamn.
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Beskriver document typen.
        /// </summary>
        public required string Description { get; init; }

        /// <summary>
        /// Anger document category.
        /// </summary>
        public DocumentCategory Category { get; init; }

        /// <summary>
        /// Anger om document typen normalt avser en specifik inventory lot.
        /// </summary>
        public bool IsLotSpecificByDefault { get; init; }

        /// <summary>
        /// Anger om document typen kan ha ett giltighetsslut.
        /// </summary>
        public bool CanExpire { get; init; }

        /// <summary>
        /// Concurrency-token från senast lästa document type.
        /// </summary>
        public uint RowVersion { get; init; }
    }
}
