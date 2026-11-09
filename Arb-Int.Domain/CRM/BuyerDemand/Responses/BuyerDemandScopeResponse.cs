namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Innehåller ett matchningsbart delområde i en köpares efterfrågan.
    /// </summary>
    public sealed class BuyerDemandScopeResponse
    {
        /// <summary>
        /// Identifierar scope-posten.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Identifierar köparens efterfrågan.
        /// </summary>
        public Guid BuyerDemandId { get; init; }

        /// <summary>
        /// Identifierar affärsrelationen som scope-posten tillhör.
        /// </summary>
        public Guid BusinessRelationshipId { get; init; }

        /// <summary>
        /// Identifierar eventuell biologisk source-claim.
        /// </summary>
        public Guid? BiologicalSourceClaimId { get; init; }

        /// <summary>
        /// Identifierar eventuellt specifikt material.
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
        /// Format så som det beskrivits av köparen.
        /// </summary>
        public string? DeclaredFormat { get; init; }

        /// <summary>
        /// Fritext för avsedd användning.
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
        /// Beskriver hur efterfrågan återkommer över tid.
        /// </summary>
        public string? RecurrenceDescription { get; init; }

        /// <summary>
        /// Anger scope-postens sorteringsordning.
        /// </summary>
        public int SortOrder { get; init; }

        /// <summary>
        /// Concurrency-token från databasen.
        /// </summary>
        public uint RowVersion { get; init; }
    }
}