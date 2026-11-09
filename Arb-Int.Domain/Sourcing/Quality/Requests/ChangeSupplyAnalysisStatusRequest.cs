namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller den nya statusen för en supply analysis.
    /// </summary>
    public sealed class ChangeSupplyAnalysisStatusRequest
    {
        /// <summary>
        /// Anger analysens nya status.
        /// </summary>
        public AnalysisStatus Status { get; init; }

        /// <summary>
        /// Concurrency-token från senast lästa analysis.
        /// </summary>
        public uint RowVersion { get; init; }
    }
}
