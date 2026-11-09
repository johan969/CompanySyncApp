namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Innehåller concurrency-data för att aktivera ett användarkonto.
    /// </summary>
    public sealed class ActivatePersonUserAccountRequest
    {
        /// <summary>
        /// Concurrency-token från senast lästa version.
        /// </summary>
        public uint RowVersion { get; init; }
    }
}