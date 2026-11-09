namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Innehåller en parts koppling till en adress och kopplingens giltighet.
    /// </summary>
    public sealed class PartyAddressResponse
    {
        /// <summary>
        /// Identifieraren för PartyAddress-kopplingen.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Parten som adressen hör till.
        /// </summary>
        public Guid PartyId { get; init; }

        /// <summary>
        /// Adressens användningsområde.
        /// </summary>
        public AddressUsageType UsageType { get; init; }

        /// <summary>
        /// Frivillig etikett, exempelvis Huvudkontor eller Fakturaadress.
        /// </summary>
        public string? Label { get; init; }

        /// <summary>
        /// Eventuell mottagar- eller attention-information.
        /// </summary>
        public string? Attention { get; init; }

        /// <summary>
        /// Anger om adressen är primär för användningsområdet.
        /// </summary>
        public bool IsPrimary { get; init; }

        /// <summary>
        /// Tidpunkt då kopplingen började gälla.
        /// </summary>
        public DateTimeOffset ValidFromUtc { get; init; }

        /// <summary>
        /// Tidpunkt då kopplingen slutade gälla.
        /// </summary>
        public DateTimeOffset? ValidToUtc { get; init; }

        /// <summary>
        /// Den normaliserade adressen.
        /// </summary>
        public required AddressResponse Address { get; init; }
    }
}