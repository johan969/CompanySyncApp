using Arb_Int.Domain.Parties;

namespace Arb_Int.Domain.User
{
    /// <summary>
    /// Beskriver en aktiv organisationskoppling för en Identity-användares domänperson.
    /// </summary>
    public sealed class UserOrganizationMembershipResponse
    {
        /// <summary>
        /// Identifierar OrganizationPerson-kopplingen.
        /// </summary>
        public Guid OrganizationPersonId { get; init; }

        /// <summary>
        /// Identifierar organisationen.
        /// </summary>
        public Guid OrganizationId { get; init; }

        /// <summary>
        /// Organisationens visningsnamn.
        /// </summary>
        public required string OrganizationName { get; init; }

        /// <summary>
        /// Anger om organisationen är markerad som intern.
        /// </summary>
        public bool? IsInternal { get; init; }

        /// <summary>
        /// Anger personens relationstyp till organisationen.
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
        /// Tidpunkt då medlemskapet började gälla.
        /// </summary>
        public DateTimeOffset? ValidFromUtc { get; init; }
    }
}
