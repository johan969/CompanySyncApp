namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller active-status för en certification scheme-version.
    /// </summary>
    public sealed class SetCertificationSchemeActiveRequest
    {
        /// <summary>
        /// Anger om scheme-versionen ska vara aktiv.
        /// </summary>
        public bool IsActive { get; init; }

        /// <summary>
        /// Concurrency-token från senast lästa scheme.
        /// </summary>
        public uint RowVersion { get; init; }
    }
}
