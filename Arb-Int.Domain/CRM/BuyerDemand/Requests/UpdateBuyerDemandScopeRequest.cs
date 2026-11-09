namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Innehåller redigerbar information för ett BuyerDemand-scope.
    /// </summary>
    public sealed class UpdateBuyerDemandScopeRequest
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

        /// <summary>
        /// Concurrency-token från senast lästa version.
        /// </summary>
        public uint RowVersion { get; init; }
    }
}