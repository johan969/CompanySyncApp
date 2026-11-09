namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Innehåller en persons relation till en organisation.
    /// </summary>
    public sealed class OrganizationPersonResponse
    {
        /// <summary>
        /// Identifierar organisationsrelationen.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Identifierar organisationen.
        /// </summary>
        public Guid OrganizationId { get; init; }

        /// <summary>
        /// Organisationens visningsnamn.
        /// </summary>
        public required string OrganizationName { get; init; }

        /// <summary>
        /// Identifierar personen.
        /// </summary>
        public Guid PersonId { get; init; }

        /// <summary>
        /// Personens visningsnamn.
        /// </summary>
        public required string PersonName { get; init; }

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
        /// Anger om personen är organisationens generella primärkontakt.
        /// </summary>
        public bool IsPrimaryContact { get; init; }

        /// <summary>
        /// Tidpunkt då organisationsrelationen började gälla.
        /// </summary>
        public DateTimeOffset? ValidFromUtc { get; init; }

        /// <summary>
        /// Tidpunkt då organisationsrelationen slutade gälla.
        /// </summary>
        public DateTimeOffset? ValidToUtc { get; init; }

        /// <summary>
        /// Interna anteckningar om organisationsrelationen.
        /// </summary>
        public string? Notes { get; init; }

        /// <summary>
        /// Funktioner som personen har inom organisationen.
        /// </summary>
        public required IReadOnlyList<ContactFunctionTypeResponse> Functions { get; init; }
    }
}