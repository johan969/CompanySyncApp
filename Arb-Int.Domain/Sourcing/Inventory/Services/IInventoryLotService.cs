using Arb_Int.Domain.Services.Common;
using FluentResults;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Hanterar inventory lot-roten, metadata, mapping, kvantiteter och status.
    /// </summary>
    public interface IInventoryLotService
    {
        /// <summary>
        /// Hämtar en lot med origins och provenance-länkar.
        /// </summary>
        Task<Result<InventoryLotResponse>> GetByIdAsync(Guid inventoryLotId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Söker, filtrerar, sorterar och paginerar lots för en supplier-relation.
        /// </summary>
        Task<Result<PagedResult<InventoryLotListResponse>>> QueryAsync(Guid supplierBusinessRelationshipId, InventoryLotQueryRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Skapar en planned inventory lot inom en supplier-relation.
        /// </summary>
        Task<Result<InventoryLotResponse>> CreateAsync(Guid supplierBusinessRelationshipId, CreateInventoryLotRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Uppdaterar lotens metadata utan att ändra lotnummer, supplier-relation, mapping, kvantiteter eller status.
        /// </summary>
        Task<Result<InventoryLotResponse>> UpdateAsync(Guid inventoryLotId, UpdateInventoryLotRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sätter offering- och materialmapping explicit.
        /// </summary>
        Task<Result<InventoryLotResponse>> SetMappingAsync(Guid inventoryLotId, SetInventoryLotMappingRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sätter lotens aktuella kvantitetsbild explicit.
        /// </summary>
        Task<Result<InventoryLotResponse>> SetQuantitiesAsync(Guid inventoryLotId, SetInventoryLotQuantitiesRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Ändrar lotens aktuella status.
        /// </summary>
        Task<Result<InventoryLotResponse>> ChangeStatusAsync(Guid inventoryLotId, ChangeInventoryLotStatusRequest request, CancellationToken cancellationToken = default);
    }
}
