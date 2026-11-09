using Arb_Int.Domain.Parties;

namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Innehåller en kontaktperson kopplad till en specifik affärsrelation.
    /// </summary>
    public sealed class BusinessRelationshipContactResponse
    {
        /// <summary>
        /// Identifierar kontaktkopplingen.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Identifierar affärsrelationen.
        /// </summary>
        public Guid BusinessRelationshipId { get; init; }

        /// <summary>
        /// Identifierar personens relation till motpartsorganisationen.
        /// </summary>
        public Guid OrganizationPersonId { get; init; }

        /// <summary>
        /// Identifierar personen.
        /// </summary>
        public Guid PersonId { get; init; }

        /// <summary>
        /// Personens visningsnamn.
        /// </summary>
        public required string PersonName { get; init; }

        /// <summary>
        /// Personens befattning hos motpartsorganisationen.
        /// </summary>
        public string? JobTitle { get; init; }

        /// <summary>
        /// Personens avdelning hos motpartsorganisationen.
        /// </summary>
        public string? Department { get; init; }

        /// <summary>
        /// Anger personens relation till organisationen.
        /// </summary>
        public OrganizationPersonRelationshipType OrganizationRelationshipType { get; init; }

        /// <summary>
        /// Tidpunkt då personens organisationsrelation började gälla.
        /// </summary>
        public DateTimeOffset? OrganizationRelationshipValidFromUtc { get; init; }

        /// <summary>
        /// Tidpunkt då personens organisationsrelation slutade gälla.
        /// </summary>
        public DateTimeOffset? OrganizationRelationshipValidToUtc { get; init; }

        /// <summary>
        /// Personens roll i den specifika affärsrelationen.
        /// </summary>
        public BusinessRelationshipContactRole Role { get; init; }

        /// <summary>
        /// Anger om personen är primärkontakt för affärsrelationen.
        /// </summary>
        public bool IsPrimary { get; init; }

        /// <summary>
        /// Interna anteckningar om kontaktens roll i relationen.
        /// </summary>
        public string? Notes { get; init; }
    }
}