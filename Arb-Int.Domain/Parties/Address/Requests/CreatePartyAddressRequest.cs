namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Innehåller data för att lägga till en adress på en Party.
    /// </summary>
    public sealed class CreatePartyAddressRequest
    {
        /// <summary>
        /// Adressen som ska kopplas till parten.
        /// </summary>
        public required AddressRequest Address { get; init; }

        /// <summary>
        /// Anger hur adressen används.
        /// </summary>
        public AddressUsageType UsageType { get; init; }

        /// <summary>
        /// Frivillig etikett för adressen.
        /// </summary>
        public string? Label { get; init; }

        /// <summary>
        /// Eventuell mottagar- eller attention-information.
        /// </summary>
        public string? Attention { get; init; }

        /// <summary>
        /// Anger om adressen ska vara primär för användningsområdet.
        /// </summary>
        public bool IsPrimary { get; init; }

        /// <summary>
        /// Tidpunkt då adresskopplingen börjar gälla. Aktuell UTC-tid används om värdet saknas.
        /// </summary>
        public DateTimeOffset? ValidFromUtc { get; init; }
    }
}