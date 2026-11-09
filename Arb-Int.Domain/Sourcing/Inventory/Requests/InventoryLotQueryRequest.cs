using Arb_Int.Domain.Services.Common;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Anger filtrering, sortering och paginering för inventory lots.
    /// </summary>
    public sealed class InventoryLotQueryRequest
    {
        /// <summary>
        /// Fritext som matchas mot lotnummer, supplier lotnummer, namn och warehouse reference.
        /// </summary>
        public string? Search { get; init; }

        /// <summary>
        /// Begränsar resultatet till en lot-status.
        /// </summary>
        public InventoryLotStatus? Status { get; init; }

        /// <summary>
        /// Begränsar resultatet till ett supplier material offering.
        /// </summary>
        public Guid? SupplierMaterialOfferingId { get; init; }

        /// <summary>
        /// Begränsar resultatet till ett kanoniskt material.
        /// </summary>
        public Guid? MaterialId { get; init; }

        /// <summary>
        /// Begränsar resultatet till nuvarande geografisk plats.
        /// </summary>
        public Guid? CurrentLocationId { get; init; }

        /// <summary>
        /// Anger sorteringsfält.
        /// </summary>
        public InventoryLotSortField SortBy { get; init; } = InventoryLotSortField.Name;

        /// <summary>
        /// Anger sorteringsriktning.
        /// </summary>
        public SortDirection SortDirection { get; init; } = SortDirection.Ascending;

        /// <summary>
        /// Sidnummer med start på 1.
        /// </summary>
        public int Page { get; init; } = 1;

        /// <summary>
        /// Maximalt antal poster per sida.
        /// </summary>
        public int PageSize { get; init; } = 50;
    }

    /// <summary>
    /// Anger tillgängliga sorteringsfält för inventory lots.
    /// </summary>
    public enum InventoryLotSortField
    {
        /// <summary>
        /// Sorterar på lotens namn.
        /// </summary>
        Name = 0,

        /// <summary>
        /// Sorterar på lotnummer.
        /// </summary>
        LotNumber = 1,

        /// <summary>
        /// Sorterar på status.
        /// </summary>
        Status = 2,

        /// <summary>
        /// Sorterar på total kvantitet.
        /// </summary>
        TotalQuantity = 3,

        /// <summary>
        /// Sorterar på tillgänglig kvantitet.
        /// </summary>
        AvailableQuantity = 4,

        /// <summary>
        /// Sorterar på best-before.
        /// </summary>
        BestBeforeUtc = 5,

        /// <summary>
        /// Sorterar på expiration.
        /// </summary>
        ExpiresAtUtc = 6,

        /// <summary>
        /// Sorterar på skapandetid.
        /// </summary>
        CreateDateUtc = 7
    }
}
