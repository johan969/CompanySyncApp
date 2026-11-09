namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Innehåller den önskade mappingen för ett biologiskt source-claim.
    /// </summary>
    public sealed class SetBiologicalSourceClaimMappingRequest
    {
        /// <summary>
        /// Identifierar den interna biologiska källan eller null om ingen källa är vald.
        /// </summary>
        public Guid? BiologicalSourceId { get; init; }

        /// <summary>
        /// Anger mappingens nya status.
        /// </summary>
        public ClaimMappingStatus MappingStatus { get; init; }

        /// <summary>
        /// Anger mappingens confidence mellan 0 och 1.
        /// </summary>
        public decimal? MappingConfidence { get; init; }
    }
}