namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller kompakt reference-data för en certification scheme-version.
    /// </summary>
    public sealed class CertificationSchemeListResponse
    {
        /// <summary>
        /// Identifierar scheme-versionen.
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// Anger scheme-versionens stabila code.
        /// </summary>
        public required string Code { get; init; }

        /// <summary>
        /// Anger scheme-versionens visningsnamn.
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Anger scheme-kategorin.
        /// </summary>
        public CertificationSchemeKind Kind { get; init; }

        /// <summary>
        /// Anger standardägaren.
        /// </summary>
        public string? StandardOwner { get; init; }

        /// <summary>
        /// Anger standardens version eller revision.
        /// </summary>
        public string? StandardVersion { get; init; }

        /// <summary>
        /// Anger när scheme-versionen börjar gälla.
        /// </summary>
        public DateTimeOffset? ValidFromUtc { get; init; }

        /// <summary>
        /// Anger när scheme-versionen slutar gälla.
        /// </summary>
        public DateTimeOffset? ValidToUtc { get; init; }

        /// <summary>
        /// Anger om scheme-versionen är aktiv.
        /// </summary>
        public bool IsActive { get; init; }

        /// <summary>
        /// Concurrency-token från databasen.
        /// </summary>
        public uint RowVersion { get; init; }
    }
}
