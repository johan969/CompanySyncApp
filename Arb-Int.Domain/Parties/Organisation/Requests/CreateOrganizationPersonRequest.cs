namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Innehåller data för att koppla en person till en organisation.
    /// </summary>
    public sealed class CreateOrganizationPersonRequest
    {
        /// <summary>
        /// Identifierar personen som ska kopplas till organisationen.
        /// </summary>
        public Guid PersonId { get; init; }

        /// <summary>
        /// Anger personens typ av relation till organisationen.
        /// </summary>
        public OrganizationPersonRelationshipType RelationshipType { get; init; }

        /// <summary>
        /// Personens befattning inom organisationen.
        /// </summary>
        public string? JobTitle { get; init; }

        /// <summary>
        /// Personens avdelning inom organisationen.
        /// </summary>
        public string? Department { get; init; }

        /// <summary>
        /// Anger om personen ska vara organisationens generella primärkontakt.
        /// </summary>
        public bool IsPrimaryContact { get; init; }

        /// <summary>
        /// Tidpunkt då relationen börjar gälla. Aktuell UTC-tid används om värdet saknas.
        /// </summary>
        public DateTimeOffset? ValidFromUtc { get; init; }

        /// <summary>
        /// Interna anteckningar om relationen.
        /// </summary>
        public string? Notes { get; init; }

        /// <summary>
        /// Funktionstyper som ska tilldelas relationen direkt vid skapandet.
        /// </summary>
        public IReadOnlyCollection<int> ContactFunctionTypeIds { get; init; } = [];
    }
}