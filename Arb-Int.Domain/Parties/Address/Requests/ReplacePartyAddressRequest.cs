namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Ersätter en aktiv adresskoppling med en ny historiserad version.
    /// </summary>
    public sealed class ReplacePartyAddressRequest
    {
        /// <summary>
        /// Den nya adressen.
        /// </summary>
        public required AddressRequest Address { get; init; }

        /// <summary>
        /// Den nya adressens användningsområde.
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
        /// Anger om den nya adressen ska vara primär.
        /// </summary>
        public bool IsPrimary { get; init; }

        /// <summary>
        /// Tidpunkt då den nya adressen börjar gälla. Aktuell UTC-tid används om värdet saknas.
        /// </summary>
        public DateTimeOffset? ValidFromUtc { get; init; }
    }
}