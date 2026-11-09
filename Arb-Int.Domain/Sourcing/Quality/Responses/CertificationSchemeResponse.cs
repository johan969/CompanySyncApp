namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller en certification scheme-version med jurisdictions.
    /// </summary>
    public sealed class CertificationSchemeResponse
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
        /// Beskriver certification scheme-versionen.
        /// </summary>
        public required string Description { get; init; }

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
        /// Anger om scheme-versionen är aktiv.
        /// </summary>
        public bool IsActive { get; init; }

        /// <summary>
        /// Innehåller jurisdictions som hör till scheme-versionen.
        /// </summary>
        public required IReadOnlyList<CertificationSchemeJurisdictionResponse> Jurisdictions { get; init; }

        /// <summary>
        /// Concurrency-token för scheme-aggregatet.
        /// </summary>
        public uint RowVersion { get; init; }
    }
}
