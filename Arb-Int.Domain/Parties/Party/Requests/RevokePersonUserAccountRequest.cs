namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Innehåller concurrency-data för att återkalla ett användarkonto.
    /// </summary>
    public sealed class RevokePersonUserAccountRequest
    {
        /// <summary>
        /// Concurrency-token från senast lästa version.
        /// </summary>
        public uint RowVersion { get; init; }
    }
}