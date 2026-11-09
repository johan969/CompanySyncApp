using Arb_Int.Domain.User;

namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Innehåller en persons koppling till ett Identity-användarkonto.
    /// </summary>
    public sealed class PersonUserAccountResponse
    {
        /// <summary>Identifierar kontokopplingen.</summary>
        public Guid Id { get; init; }
        /// <summary>Identifierar domänpersonen.</summary>
        public Guid PersonId { get; init; }
        /// <summary>Personens visningsnamn.</summary>
        public required string PersonName { get; init; }
        /// <summary>Identifierar Identity-användaren.</summary>
        public Guid ApplicationUserId { get; init; }
        /// <summary>Identifierar vald auth-e-postkontaktpunkt.</summary>
        public Guid? AuthEmailContactPointId { get; init; }
        /// <summary>Auth-e-postadressen från vald kontaktpunkt.</summary>
        public string? AuthEmail { get; init; }
        /// <summary>Kontokopplingens aktuella status.</summary>
        public PersonUserAccountStatus Status { get; init; }
        /// <summary>Anger om användaren är extern.</summary>
        public bool IsExternalUser { get; init; }
        /// <summary>Tidpunkt då användaren bjöds in.</summary>
        public DateTimeOffset? InvitedAtUtc { get; init; }
        /// <summary>Tidpunkt då användaren aktiverades.</summary>
        public DateTimeOffset? ActivatedAtUtc { get; init; }
        /// <summary>Tidpunkt då användarens åtkomst återkallades.</summary>
        public DateTimeOffset? RevokedAtUtc { get; init; }
        /// <summary>Identifierar användaren som skickade invitationen.</summary>
        public Guid? InvitedByUserId { get; init; }
        /// <summary>Interna anteckningar om kontokopplingen.</summary>
        public string? Notes { get; init; }
        /// <summary>Tidpunkt då kontokopplingen skapades.</summary>
        public DateTimeOffset? CreateDateUtc { get; init; }
        /// <summary>Concurrency-token som ska skickas tillbaka vid ändringar.</summary>
        public uint RowVersion { get; init; }
    }
}
