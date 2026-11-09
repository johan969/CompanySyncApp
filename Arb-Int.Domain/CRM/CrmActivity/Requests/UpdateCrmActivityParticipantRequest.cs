namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Innehåller redigerbar information för en deltagare i en CRM-aktivitet.
    /// </summary>
    public sealed class UpdateCrmActivityParticipantRequest
    {
        /// <summary>
        /// Anger deltagarens roll i aktiviteten.
        /// </summary>
        public CrmActivityParticipantRole Role { get; init; }
    }
}