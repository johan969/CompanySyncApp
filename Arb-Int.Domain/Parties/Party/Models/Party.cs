using Arb_Int.Domain.Base;
using Arb_Int.Domain.Worker;

namespace Arb_Int.Domain.Parties
{
    public abstract class Party :
        IEntity<Guid>,
        INamed,
        IDescribable,
        ICreateTracker,
        IActionTracker,
        ISystemVersioned,
        INormalizedName
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Partens primära visningsnamn.
        ///
        /// För en organisation kan detta vara handelsnamnet.
        /// För en person kan detta vara det fullständiga namnet.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Normaliserad variant av Name för sökning och dubblettkontroll.
        /// </summary>
        public string NormalizedName { get; set; } = null!;

        public string Description { get; set; } = string.Empty;

        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTimeOffset? CreateDateUtc { get; set; }

        public string? ActionName { get; set; }

        public DateTimeOffset? ActionDateUtc { get; set; }

        public Guid? ActionUserId { get; set; }

        public ICollection<PartyAddress> AddressLinks { get; set; } = [];

        public ICollection<PartyContactPoint> ContactPoints { get; set; } = [];

        public ICollection<PartyExternalReference> ExternalReferences { get; set; } = [];

        public ICollection<ScheduledTask> ScheduledTaskLinks { get; set; } = [];
    }
}