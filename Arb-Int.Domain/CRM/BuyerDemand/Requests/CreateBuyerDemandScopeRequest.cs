namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Innehåller data för att lägga till ett scope på en BuyerDemand.
    /// </summary>
    public sealed class CreateBuyerDemandScopeRequest
    {
        /// <summary>
        /// Identifierar eventuell biologisk source-claim.
        /// </summary>
        public Guid? BiologicalSourceClaimId { get; init; }

        /// <summary>
        /// Identifierar eventuellt material.
        /// </summary>
        public Guid? MaterialId { get; init; }

        /// <summary>
        /// Identifierar eventuell materialtyp.
        /// </summary>
        public int? MaterialTypeId { get; init; }

        /// <summary>
        /// Identifierar eventuell avsedd användningskaraktäristik.
        /// </summary>
        public Guid? IntendedUseCharacteristicId { get; init; }

        /// <summary>
        /// Köparens deklarerade format.
        /// </summary>
        public string? DeclaredFormat { get; init; }

        /// <summary>
        /// Köparens beskrivning av avsedd användning.
        /// </summary>
        public string? IntendedUseText { get; init; }

        /// <summary>
        /// Typisk efterfrågad kvantitet.
        /// </summary>
        public decimal? TypicalQuantity { get; init; }

        /// <summary>
        /// Enhet för den typiska kvantiteten.
        /// </summary>
        public int? TypicalQuantityUnitId { get; init; }

        /// <summary>
        /// Beskriver hur efterfrågan återkommer.
        /// </summary>
        public string? RecurrenceDescription { get; init; }

        /// <summary>
        /// Anger sorteringsordning.
        /// </summary>
        public int SortOrder { get; init; }
    }
}