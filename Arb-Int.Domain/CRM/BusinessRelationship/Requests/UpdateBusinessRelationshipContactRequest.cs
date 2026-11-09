namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Innehåller redigerbar information för en kontakt på en affärsrelation.
    /// </summary>
    public sealed class UpdateBusinessRelationshipContactRequest
    {
        /// <summary>
        /// Personens roll i den specifika affärsrelationen.
        /// </summary>
        public BusinessRelationshipContactRole Role { get; init; }

        /// <summary>
        /// Anger om personen ska vara affärsrelationens primärkontakt.
        /// </summary>
        public bool IsPrimary { get; init; }

        /// <summary>
        /// Interna anteckningar om kontaktens roll.
        /// </summary>
        public string? Notes { get; init; }
    }
}