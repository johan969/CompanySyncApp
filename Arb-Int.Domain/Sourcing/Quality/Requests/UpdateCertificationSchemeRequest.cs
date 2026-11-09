namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller redigerbar presentation och validity för en certification scheme-version.
    /// </summary>
    public sealed class UpdateCertificationSchemeRequest
    {
        /// <summary>
        /// Anger scheme-versionens visningsnamn.
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Beskriver certification scheme-versionen.
        /// </summary>
        public required string Description { get; init; }

        /// <summary>
        /// Anger publik informationslänk när sådan finns.
        /// </summary>
        public string? PublicUrl { get; init; }

        /// <summary>
        /// Anger när scheme-versionen börjar gälla.
        /// </summary>
        public DateTimeOffset? ValidFromUtc { get; init; }

        /// <summary>
        /// Anger när scheme-versionen slutar gälla.
        /// </summary>
        public DateTimeOffset? ValidToUtc { get; init; }

        /// <summary>
        /// Concurrency-token från senast lästa scheme.
        /// </summary>
        public uint RowVersion { get; init; }
    }
}
