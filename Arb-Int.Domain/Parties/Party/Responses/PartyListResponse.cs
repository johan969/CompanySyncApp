namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Innehåller den kompakta Party-information som används i listor och sökresultat.
    /// </summary>
    public sealed class PartyListResponse
    {
        /// <summary>
        /// Partens identifierare.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Partens visningsnamn.
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Anger vilken konkret Party-typ posten representerar.
        /// </summary>
        public PartyType PartyType { get; init; }

        /// <summary>
        /// Anger om parten är aktiv.
        /// </summary>
        public bool IsActive { get; init; }

        /// <summary>
        /// Organisationens juridiska namn när parten är en organisation.
        /// </summary>
        public string? LegalName { get; init; }

        /// <summary>
        /// Organisationens handelsnamn när parten är en organisation.
        /// </summary>
        public string? TradingName { get; init; }

        /// <summary>
        /// Organisationstyp när parten är en organisation.
        /// </summary>
        public OrganizationKind? OrganizationKind { get; init; }

        /// <summary>
        /// Personens förnamn när parten är en person.
        /// </summary>
        public string? GivenName { get; init; }

        /// <summary>
        /// Personens efternamn när parten är en person.
        /// </summary>
        public string? FamilyName { get; init; }

        /// <summary>
        /// Tidpunkt då parten skapades.
        /// </summary>
        public DateTimeOffset? CreateDateUtc { get; init; }
    }
}