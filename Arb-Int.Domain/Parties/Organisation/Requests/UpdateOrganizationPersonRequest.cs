namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Innehåller redigerbar information för en aktiv organisationsrelation.
    /// </summary>
    public sealed class UpdateOrganizationPersonRequest
    {
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
        /// Interna anteckningar om relationen.
        /// </summary>
        public string? Notes { get; init; }
    }
}