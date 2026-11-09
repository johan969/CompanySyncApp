namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Innehåller sluttiden för en kontaktpunkt.
    /// </summary>
    public sealed class EndPartyContactPointRequest
    {
        /// <summary>
        /// Tidpunkt då kontaktpunkten slutade gälla.
        /// </summary>
        public DateTimeOffset ValidToUtc { get; init; }
    }
}