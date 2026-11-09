namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Innehåller personspecifik information för en Party.
    /// </summary>
    public sealed class PersonPartyResponse
    {
        /// <summary>
        /// Personens förnamn.
        /// </summary>
        public required string GivenName { get; init; }

        /// <summary>
        /// Personens mellannamn.
        /// </summary>
        public string? MiddleName { get; init; }

        /// <summary>
        /// Personens efternamn.
        /// </summary>
        public required string FamilyName { get; init; }

        /// <summary>
        /// Personens föredragna namn.
        /// </summary>
        public string? PreferredName { get; init; }

        /// <summary>
        /// Personens titel, exempelvis Dr eller Prof.
        /// </summary>
        public string? Honorific { get; init; }

        /// <summary>
        /// Föredragen BCP-47-språkkod.
        /// </summary>
        public string? PreferredLanguageCode { get; init; }

        /// <summary>
        /// Antal organisationsrelationer för personen.
        /// </summary>
        public int OrganizationLinkCount { get; init; }

        /// <summary>
        /// Anger om personen är kopplad till ett användarkonto.
        /// </summary>
        public bool HasUserAccount { get; init; }
    }
}