namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Innehåller data för att lägga till en identifierare på en organisation.
    /// </summary>
    public sealed class CreateOrganizationIdentifierRequest
    {
        /// <summary>
        /// Maskinkod för identifierartypen, exempelvis organization-number, vat, lei, duns eller eori.
        /// </summary>
        public required string Scheme { get; init; }

        /// <summary>
        /// Identifierarens värde i dess ursprungliga presentationsformat.
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
        /// Anger om identifieraren ska vara primär för organisationen och aktuell scheme.
        /// </summary>
        public bool IsPrimary { get; init; }
    }
}