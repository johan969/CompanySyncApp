namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Innehåller detaljerad information om en Party.
    /// </summary>
    public sealed class PartyDetailsResponse
    {
        /// <summary>
        /// Partens identifierare.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Partens primära visningsnamn.
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Partens beskrivning.
        /// </summary>
        public required string Description { get; init; }

        /// <summary>
        /// Interna anteckningar om parten.
        /// </summary>
        public string? Notes { get; init; }

        /// <summary>
        /// Anger om parten är aktiv.
        /// </summary>
        public bool IsActive { get; init; }

        /// <summary>
        /// Anger vilken konkret Party-typ posten representerar.
        /// </summary>
        public PartyType PartyType { get; init; }

        /// <summary>
        /// Tidpunkt då parten skapades.
        /// </summary>
        public DateTimeOffset? CreateDateUtc { get; init; }

        /// <summary>
        /// Organisationsinformation när parten är en organisation.
        /// </summary>
        public OrganizationPartyResponse? Organization { get; init; }

        /// <summary>
        /// Personinformation när parten är en person.
        /// </summary>
        public PersonPartyResponse? Person { get; init; }

        /// <summary>
        /// Antal adresser kopplade till parten.
        /// </summary>
        public int AddressCount { get; init; }

        /// <summary>
        /// Antal kontaktpunkter kopplade till parten.
        /// </summary>
        public int ContactPointCount { get; init; }

        /// <summary>
        /// Antal externa referenser kopplade till parten.
        /// </summary>
        public int ExternalReferenceCount { get; init; }
    }
}