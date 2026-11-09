namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller data för att lägga till en jurisdiction på en certification scheme-version.
    /// </summary>
    public sealed class CreateCertificationSchemeJurisdictionRequest
    {
        /// <summary>
        /// Anger jurisdiction-typen.
        /// </summary>
        public CertificationJurisdictionKind Kind { get; init; }

        /// <summary>
        /// Anger stabil code för jurisdictionen inom scheme-versionen.
        /// </summary>
        public required string Code { get; init; }

        /// <summary>
        /// Anger jurisdictionens visningsnamn.
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Identifierar country när jurisdictionen har en kanonisk country-koppling.
        /// </summary>
        public Guid? CountryId { get; init; }

        /// <summary>
        /// Innehåller anteckningar om jurisdictionen.
        /// </summary>
        public string? Notes { get; init; }

        /// <summary>
        /// Concurrency-token för certification scheme-aggregatet.
        /// </summary>
        public uint CertificationSchemeRowVersion { get; init; }
    }
}
