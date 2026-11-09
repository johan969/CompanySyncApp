namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller data för att skapa en certification scheme-definition.
    /// </summary>
    public sealed class CreateCertificationSchemeRequest
    {
        /// <summary>
        /// Anger stabil kod för scheme-versionen.
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
        /// Anger organisationen eller standardägaren bakom scheme-versionen.
        /// </summary>
        public required string StandardOwner { get; init; }

        /// <summary>
        /// Anger standardens version eller revision.
        /// </summary>
        public required string StandardVersion { get; init; }

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
    }
}
