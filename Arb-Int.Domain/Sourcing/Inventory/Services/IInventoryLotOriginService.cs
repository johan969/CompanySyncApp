using FluentResults;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Hanterar geografiska origin-poster för inventory lots.
    /// </summary>
    public interface IInventoryLotOriginService
    {
        /// <summary>
        /// Lägger till en origin-post och returnerar den uppdaterade loten.
        /// </summary>
        Task<Result<InventoryLotResponse>> AddAsync(Guid inventoryLotId, CreateInventoryLotOriginRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Uppdaterar roll, verifiering, primary-status, proportion och anteckningar utan att byta geografisk plats.
        /// </summary>
        Task<Result<InventoryLotResponse>> UpdateAsync(Guid inventoryLotOriginId, UpdateInventoryLotOriginRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Tar bort en origin-post utan att ta bort GeographicLocation.
        /// </summary>
        Task<Result<InventoryLotResponse>> DeleteAsync(Guid inventoryLotOriginId, uint rowVersion, uint inventoryLotRowVersion, CancellationToken cancellationToken = default);
    }
}
