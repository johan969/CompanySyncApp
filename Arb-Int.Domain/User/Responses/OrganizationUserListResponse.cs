using Arb_Int.Domain.Parties;

namespace Arb_Int.Domain.User
{
    /// <summary>
    /// Innehåller en organisationsperson tillsammans med eventuell Identity- och PersonUserAccount-information.
    /// </summary>
    public sealed class OrganizationUserListResponse
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
        /// Identifierar domänpersonen.
        /// </summary>
        public Guid PersonId { get; init; }

        /// <summary>
        /// Personens visningsnamn.
        /// </summary>
        public required string PersonName { get; init; }

        /// <summary>
        /// Personens relation till organisationen.
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
        /// Tidpunkt då organisationskopplingen började gälla.
        /// </summary>
        public DateTimeOffset? ValidFromUtc { get; init; }

        /// <summary>
        /// Tidpunkt då organisationskopplingen avslutades.
        /// </summary>
        public DateTimeOffset? ValidToUtc { get; init; }

        /// <summary>
        /// Anger om personen har en PersonUserAccount-koppling.
        /// </summary>
        public bool HasUserAccount { get; init; }

        /// <summary>
        /// Identifierar PersonUserAccount-kopplingen när sådan finns.
        /// </summary>
        public Guid? PersonUserAccountId { get; init; }

        /// <summary>
        /// Identifierar Identity-användaren när sådan finns.
        /// </summary>
        public Guid? ApplicationUserId { get; init; }

        /// <summary>
        /// PersonUserAccount-kopplingens status när sådan finns.
        /// </summary>
        public PersonUserAccountStatus? AccountStatus { get; init; }

        /// <summary>
        /// Anger om användarkontokopplingen är extern.
        /// </summary>
        public bool? IsExternalUser { get; init; }

        /// <summary>
        /// Identifierar vald PartyContactPoint för Identity-kontots auth-e-postadress.
        /// </summary>
        public Guid? AuthEmailContactPointId { get; init; }

        /// <summary>
        /// Identity-kontots e-postadress när sådan finns.
        /// </summary>
        public string? Email { get; init; }

        /// <summary>
        /// Identity-kontots användarnamn när sådan finns.
        /// </summary>
        public string? UserName { get; init; }

        /// <summary>
        /// Anger om Identity-kontots e-postadress är bekräftad.
        /// </summary>
        public bool? IsEmailConfirmed { get; init; }

        /// <summary>
        /// Anger om Identity-kontot är inaktiverat.
        /// </summary>
        public bool? IsInactivated { get; init; }

        /// <summary>
        /// Tidpunkt då användaren bjöds in.
        /// </summary>
        public DateTimeOffset? InvitedAtUtc { get; init; }

        /// <summary>
        /// Tidpunkt då användarkontokopplingen aktiverades.
        /// </summary>
        public DateTimeOffset? ActivatedAtUtc { get; init; }

        /// <summary>
        /// Tidpunkt då användarkontokopplingen återkallades.
        /// </summary>
        public DateTimeOffset? RevokedAtUtc { get; init; }
    }
}
