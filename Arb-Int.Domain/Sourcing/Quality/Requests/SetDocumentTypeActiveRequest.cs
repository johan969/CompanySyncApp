namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller active-status för en document type.
    /// </summary>
    public sealed class SetDocumentTypeActiveRequest
    {
        /// <summary>
        /// Anger om document typen ska vara aktiv.
        /// </summary>
        public bool IsActive { get; init; }

        /// <summary>
        /// Concurrency-token från senast lästa document type.
        /// </summary>
        public uint RowVersion { get; init; }
    }
}
