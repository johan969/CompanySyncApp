namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Innehåller verifieringsstatus för ett biologiskt source-claim.
    /// </summary>
    public sealed class SetBiologicalSourceClaimVerificationRequest
    {
        /// <summary>
        /// Anger claimens nya verifieringsstatus.
        /// </summary>
        public ClaimVerificationStatus VerificationStatus { get; init; }

        /// <summary>
        /// Tidpunkt då claimen senast verifierades eller null om ingen verifieringstid ska anges.
        /// </summary>
        public DateTimeOffset? LastVerifiedUtc { get; init; }
    }
}