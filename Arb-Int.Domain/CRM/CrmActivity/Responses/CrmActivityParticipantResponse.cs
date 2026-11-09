namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Innehåller en deltagare i en CRM-aktivitet.
    /// </summary>
    public sealed class CrmActivityParticipantResponse
    {
        /// <summary>
        /// Identifierar deltagarkopplingen.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Identifierar CRM-aktiviteten.
        /// </summary>
        public Guid CrmActivityId { get; init; }

        /// <summary>
        /// Identifierar deltagande Party.
        /// </summary>
        public Guid PartyId { get; init; }

        /// <summary>
        /// Deltagarens visningsnamn.
        /// </summary>
        public required string PartyName { get; init; }

        /// <summary>
        /// Anger deltagarens roll i aktiviteten.
        /// </summary>
        public CrmActivityParticipantRole Role { get; init; }
    }
}