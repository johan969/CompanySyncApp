using Arb_Int.Domain.Base;
using Arb_Int.Domain.User;

namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Kopplar en Person till Identity först när personen ska kunna logga in. Domänpersonen förblir separat från ApplicationUser.
    /// </summary>
    [SystemHistoryPolicy(SystemHistoryPolicy.PseudonymizeOnErasure)]
    public class PersonUserAccount : IEntity<Guid>, ICreateTracker, IActionTracker, IRowVersion, ISystemVersioned
    {
        /// <summary>Identifierar kontokopplingen.</summary>
        public Guid Id { get; set; }
        /// <summary>Identifierar domänpersonen.</summary>
        public Guid PersonId { get; set; }
        /// <summary>Identifierar Identity-användaren.</summary>
        public Guid ApplicationUserId { get; set; }
        /// <summary>Identifierar den aktiva e-postkontaktpunkt som används som Identity-kontots auth-adress.</summary>
        public Guid? AuthEmailContactPointId { get; set; }
        /// <summary>Anger kontokopplingens aktuella status.</summary>
        public PersonUserAccountStatus Status { get; set; }
        /// <summary>Anger om användaren är extern.</summary>
        public bool IsExternalUser { get; set; }
        /// <summary>Tidpunkt då användaren bjöds in.</summary>
        public DateTimeOffset? InvitedAtUtc { get; set; }
        /// <summary>Tidpunkt då kontot aktiverades.</summary>
        public DateTimeOffset? ActivatedAtUtc { get; set; }
        /// <summary>Tidpunkt då åtkomsten återkallades.</summary>
        public DateTimeOffset? RevokedAtUtc { get; set; }
        /// <summary>Identifierar användaren som skickade invitationen.</summary>
        public Guid? InvitedByUserId { get; set; }
        /// <summary>Interna anteckningar om kontokopplingen.</summary>
        public string? Notes { get; set; }
        /// <summary>Tidpunkt då kontokopplingen skapades.</summary>
        public DateTimeOffset? CreateDateUtc { get; set; }
        /// <summary>Senaste actionsnamn.</summary>
        public string? ActionName { get; set; }
        /// <summary>Tidpunkt för senaste action.</summary>
        public DateTimeOffset? ActionDateUtc { get; set; }
        /// <summary>Identifierar användaren bakom senaste action.</summary>
        public Guid? ActionUserId { get; set; }
        /// <summary>Optimistic concurrency-token.</summary>
        public uint RowVersion { get; set; }
        /// <summary>Domänpersonen.</summary>
        public Person Person { get; set; } = null!;
        /// <summary>Identity-användaren.</summary>
        public ApplicationUser ApplicationUser { get; set; } = null!;
        /// <summary>Vald auth-e-postkontaktpunkt.</summary>
        public PartyContactPoint? AuthEmailContactPoint { get; set; }
    }
}
