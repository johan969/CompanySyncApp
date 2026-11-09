namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Innehåller en extern eller officiell identifierare för en organisation.
    /// </summary>
    public sealed class OrganizationIdentifierResponse
    {
        /// <summary>
        /// Identifierar posten.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Identifierar organisationen som identifieraren tillhör.
        /// </summary>
        public Guid OrganizationId { get; init; }

        /// <summary>
        /// Maskinkod för identifierartypen, exempelvis organization-number, vat, lei, duns eller eori.
        /// </summary>
        public required string Scheme { get; init; }

        /// <summary>
        /// Identifierarens ursprungliga värde.
        /// </summary>
        public required string Value { get; init; }

        /// <summary>
        /// Eventuell utfärdande organisation eller myndighet.
        /// </summary>
        public string? Issuer { get; init; }

        /// <summary>
        /// Identifierar eventuellt utfärdande land.
        /// </summary>
        public Guid? IssuingCountryId { get; init; }

        /// <summary>
        /// Anger om identifieraren är primär för organisationen och aktuell scheme.
        /// </summary>
        public bool IsPrimary { get; init; }
    }
}