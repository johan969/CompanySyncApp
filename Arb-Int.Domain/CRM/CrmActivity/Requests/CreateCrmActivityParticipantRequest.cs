namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Innehåller data för att lägga till en deltagare på en CRM-aktivitet.
    /// </summary>
    public sealed class CreateCrmActivityParticipantRequest
    {
        /// <summary>
        /// Identifierar deltagande Party.
        /// </summary>
        public Guid PartyId { get; init; }

        /// <summary>
        /// Anger deltagarens roll i aktiviteten.
        /// </summary>
        public CrmActivityParticipantRole Role { get; init; }
    }
}