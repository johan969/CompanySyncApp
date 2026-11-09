namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Innehåller redigerbar metadata för en extern Party-referens.
    /// </summary>
    public sealed class UpdatePartyExternalReferenceRequest
    {
        /// <summary>
        /// Eventuell URL till posten i källsystemet.
        /// </summary>
        public string? SourceUrl { get; init; }

        /// <summary>
        /// Tidpunkt då posten först observerades.
        /// </summary>
        public DateTimeOffset FirstSeenUtc { get; init; }

        /// <summary>
        /// Tidpunkt då posten senast observerades.
        /// </summary>
        public DateTimeOffset LastSeenUtc { get; init; }

        /// <summary>
        /// Eventuell hash av senast observerat innehåll.
        /// </summary>
        public string? ContentHash { get; init; }
    }
}