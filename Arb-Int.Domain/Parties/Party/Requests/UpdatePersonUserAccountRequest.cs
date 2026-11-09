namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Innehåller redigerbar metadata för en användarkontokoppling.
    /// </summary>
    public sealed class UpdatePersonUserAccountRequest
    {
        /// <summary>
        /// Anger om användaren är extern.
        /// </summary>
        public bool IsExternalUser { get; init; }

        /// <summary>
        /// Interna anteckningar om kontokopplingen.
        /// </summary>
        public string? Notes { get; init; }

        /// <summary>
        /// Concurrency-token från senast lästa version.
        /// </summary>
        public uint RowVersion { get; init; }
    }
}