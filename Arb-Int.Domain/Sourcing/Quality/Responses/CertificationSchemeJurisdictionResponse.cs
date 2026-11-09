namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller en jurisdiction som hör till en certification scheme-version.
    /// </summary>
    public sealed class CertificationSchemeJurisdictionResponse
    {
        /// <summary>
        /// Identifierar jurisdiction-posten.
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// Identifierar certification scheme-versionen.
        /// </summary>
        public int CertificationSchemeId { get; init; }

        /// <summary>
        /// Anger jurisdiction-typen.
        /// </summary>
        public CertificationJurisdictionKind Kind { get; init; }

        /// <summary>
        /// Anger stabil code för jurisdiktionen.
        /// </summary>
        public required string Code { get; init; }

        /// <summary>
        /// Anger jurisdictionens visningsnamn.
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Identifierar country när jurisdictionen är kopplad till ett land.
        /// </summary>
        public Guid? CountryId { get; init; }

        /// <summary>
        /// Innehåller anteckningar om jurisdictionen.
        /// </summary>
        public string? Notes { get; init; }

        /// <summary>
        /// Concurrency-token från databasen.
        /// </summary>
        public uint RowVersion { get; init; }
    }
}
