using FluentResults;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Hanterar provenance-länkar mellan inventory lots.
    /// </summary>
    public interface IInventoryLotLinkService
    {
        /// <summary>
        /// Skapar en riktad provenance-länk från parent-lot till child-lot.
        /// </summary>
        Task<Result<InventoryLotResponse>> AddAsync(Guid childInventoryLotId, CreateInventoryLotLinkRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Uppdaterar länkkind och provenance-metadata utan att byta parent eller child.
        /// </summary>
        Task<Result<InventoryLotResponse>> UpdateAsync(Guid inventoryLotLinkId, UpdateInventoryLotLinkRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Tar bort provenance-länken och returnerar child-loten efter ändringen.
        /// </summary>
        Task<Result<InventoryLotResponse>> DeleteAsync(Guid inventoryLotLinkId, uint rowVersion, uint parentInventoryLotRowVersion, uint childInventoryLotRowVersion, CancellationToken cancellationToken = default);
    }
}
