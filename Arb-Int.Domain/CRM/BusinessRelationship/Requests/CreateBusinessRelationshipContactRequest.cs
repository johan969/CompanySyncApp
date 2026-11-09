namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Innehåller data för att lägga till en kontaktperson på en affärsrelation.
    /// </summary>
    public sealed class CreateBusinessRelationshipContactRequest
    {
        /// <summary>
        /// Identifierar personens relation till motpartsorganisationen.
        /// </summary>
        public Guid OrganizationPersonId { get; init; }

        /// <summary>
        /// Personens roll i den specifika affärsrelationen.
        /// </summary>
        public BusinessRelationshipContactRole Role { get; init; }

        /// <summary>
        /// Anger om personen ska vara affärsrelationens primärkontakt.
        /// </summary>
        public bool IsPrimary { get; init; }

        /// <summary>
        /// Interna anteckningar om kontaktens roll.
        /// </summary>
        public string? Notes { get; init; }
    }
}