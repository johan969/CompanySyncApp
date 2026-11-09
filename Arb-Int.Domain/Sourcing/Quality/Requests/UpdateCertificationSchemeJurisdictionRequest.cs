namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller redigerbar metadata för en certification scheme jurisdiction.
    /// </summary>
    public sealed class UpdateCertificationSchemeJurisdictionRequest
    {
        /// <summary>
        /// Anger jurisdiction-typen.
        /// </summary>
        public CertificationJurisdictionKind Kind { get; init; }

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
        /// Concurrency-token för jurisdiction-raden.
        /// </summary>
        public uint RowVersion { get; init; }

        /// <summary>
        /// Concurrency-token för certification scheme-aggregatet.
        /// </summary>
        public uint CertificationSchemeRowVersion { get; init; }
    }
}
