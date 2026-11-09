namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Innehåller de värden som ska ersätta en persons redigerbara grunddata.
    /// </summary>
    public sealed class UpdatePersonRequest
    {
        /// <summary>
        /// Personens primära visningsnamn.
        /// </summary>
        public required string Name { get; init; }

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
        /// Personens beskrivning.
        /// </summary>
        public string Description { get; init; } = string.Empty;

        /// <summary>
        /// Interna anteckningar om personen.
        /// </summary>
        public string? Notes { get; init; }

        /// <summary>
        /// Anger om personen är aktiv.
        /// </summary>
        public bool IsActive { get; init; } = true;
    }
}