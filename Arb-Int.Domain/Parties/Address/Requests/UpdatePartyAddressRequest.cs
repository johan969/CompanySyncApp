namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Innehåller redigerbar metadata för en aktiv PartyAddress-koppling.
    /// </summary>
    public sealed class UpdatePartyAddressRequest
    {
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
    }
}