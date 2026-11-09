using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Parties
{
    public class PartyAddress : IEntity<Guid>, ISystemVersioned
    {
        public Guid Id { get; set; }

        public Guid PartyId { get; set; }
        public Party Party { get; set; } = null!;

        public Guid AddressId { get; set; }
        public Address Address { get; set; } = null!;

        public AddressUsageType UsageType { get; set; }

        /// <summary>
        /// Exempelvis "Huvudkontor", "Odling Norge" eller "Fakturaadress".
        /// </summary>
        public string? Label { get; set; }

        public string? Attention { get; set; }

        public bool IsPrimary { get; set; }

        public DateTimeOffset ValidFromUtc { get; set; }

        public DateTimeOffset? ValidToUtc { get; set; }
    }
}