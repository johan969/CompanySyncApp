using Arb_Int.Domain.Parties;

namespace Arb_Int.Domain.User
{
    /// <summary>
    /// Innehåller Identity-profil, domänperson och aktiva organisationsmedlemskap för en användare.
    /// </summary>
    public sealed class UserDetailsResponse
    {
        /// <summary>
        /// Identifierar Identity-användaren.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Användarens förnamn i Identity-profilen.
        /// </summary>
        public string? FirstName { get; init; }

        /// <summary>
        /// Användarens efternamn i Identity-profilen.
        /// </summary>
        public string? LastName { get; init; }

        /// <summary>
        /// Identity-användarnamnet.
        /// </summary>
        public required string UserName { get; init; }

        /// <summary>
        /// Identity-kontots e-postadress.
        /// </summary>
        public required string Email { get; init; }
        /// <summary>
        /// Identifierar vald PartyContactPoint för Identity-kontots auth-e-postadress när en domänperson finns.
        /// </summary>
        public Guid? AuthEmailContactPointId { get; init; }


        /// <summary>
        /// Anger om tvåstegsverifiering är aktiverad.
        /// </summary>
        public bool Is2StepVerificationRequired { get; init; }

        /// <summary>
        /// Anger om användaren får ändra tvåstegsverifiering via klienten.
        /// </summary>
        public bool CanToggle2StepVerification { get; init; }

        /// <summary>
        /// Anger om e-postadressen är bekräftad.
        /// </summary>
        public bool IsEmailConfirmed { get; init; }

        /// <summary>
        /// Anger om användaren måste byta lösenord innan normal användning.
        /// </summary>
        public bool IsPasswordChangeRequired { get; init; }

        /// <summary>
        /// Anger om Identity-kontot är inaktiverat.
        /// </summary>
        public bool IsInactivated { get; init; }

        /// <summary>
        /// Identifierar den kopplade domänpersonen när sådan finns.
        /// </summary>
        public Guid? PersonId { get; init; }

        /// <summary>
        /// Identifierar PersonUserAccount-kopplingen när sådan finns.
        /// </summary>
        public Guid? PersonUserAccountId { get; init; }

        /// <summary>
        /// Anger PersonUserAccount-kopplingens status när sådan finns.
        /// </summary>
        public PersonUserAccountStatus? PersonUserAccountStatus { get; init; }

        /// <summary>
        /// Anger om PersonUserAccount-kopplingen är markerad som extern.
        /// </summary>
        public bool? IsExternalUser { get; init; }

        /// <summary>
        /// Personens föredragna BCP-47-språkkod när en domänperson finns.
        /// </summary>
        public string? PreferredLanguageCode { get; init; }

        /// <summary>
        /// Anger om Identity lockout är aktiverat för kontot.
        /// </summary>
        public bool LockoutEnabled { get; init; }

        /// <summary>
        /// Tidpunkt då en eventuell lockout upphör.
        /// </summary>
        public DateTimeOffset? LockoutEnd { get; init; }

        /// <summary>
        /// Användarens telefonnummer.
        /// </summary>
        public string? PhoneNumber { get; init; }

        /// <summary>
        /// Anger om telefonnumret är bekräftat.
        /// </summary>
        public bool IsPhoneNumberConfirmed { get; init; }

        /// <summary>
        /// Antal misslyckade Identity-inloggningsförsök.
        /// </summary>
        public int AccessFailedCount { get; init; }

        /// <summary>
        /// Aktiva organisationer som användarens domänperson tillhör.
        /// </summary>
        public IReadOnlyList<UserOrganizationMembershipResponse> Organizations { get; init; } = [];

        /// <summary>
        /// Effektiva Identity- och rollclaims som hämtats för användaren.
        /// </summary>
        public IReadOnlyList<UserClaim> Claims { get; init; } = [];
    }
}
