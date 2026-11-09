namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Innehåller sluttiden för en organisationsrelation.
    /// </summary>
    public sealed class EndOrganizationPersonRequest
    {
        /// <summary>
        /// Tidpunkt då relationen slutade gälla.
        /// </summary>
        public DateTimeOffset ValidToUtc { get; init; }
    }
}