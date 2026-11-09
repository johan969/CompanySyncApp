namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Innehåller sluttiden för en aktiv PartyAddress-koppling.
    /// </summary>
    public sealed class EndPartyAddressRequest
    {
        /// <summary>
        /// Tidpunkt då adresskopplingen slutade gälla.
        /// </summary>
        public DateTimeOffset ValidToUtc { get; init; }
    }
}