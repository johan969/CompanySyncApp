namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller reference-data för en document type.
    /// </summary>
    public sealed class DocumentTypeResponse
    {
        /// <summary>
        /// Identifierar document typen.
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// Anger document typens stabila kod.
        /// </summary>
        public required string Code { get; init; }

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
        /// Anger om typen normalt avser en specifik inventory lot.
        /// </summary>
        public bool IsLotSpecificByDefault { get; init; }

        /// <summary>
        /// Anger om typen kan ha ett giltighetsslut.
        /// </summary>
        public bool CanExpire { get; init; }

        /// <summary>
        /// Anger om reference-posten är aktiv.
        /// </summary>
        public bool IsActive { get; init; }

        /// <summary>
        /// Concurrency-token från databasen.
        /// </summary>
        public uint RowVersion { get; init; }
    }
}
