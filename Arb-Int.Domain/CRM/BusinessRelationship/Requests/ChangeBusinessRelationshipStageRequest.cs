namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Innehåller data för att byta steg på en affärsrelation.
    /// </summary>
    public sealed class ChangeBusinessRelationshipStageRequest
    {
        /// <summary>
        /// Identifierar det nya steget.
        /// </summary>
        public int BusinessRelationshipStageId { get; init; }

        /// <summary>
        /// Tidpunkt då stegbytet skedde. Aktuell UTC-tid används om värdet saknas.
        /// </summary>
        public DateTimeOffset? ChangedUtc { get; init; }

        /// <summary>
        /// Identifierar användaren som gjorde stegbytet.
        /// </summary>
        public Guid? ChangedByUserId { get; init; }

        /// <summary>
        /// Anteckningar om varför steget ändrades.
        /// </summary>
        public string? Notes { get; init; }
    }
}