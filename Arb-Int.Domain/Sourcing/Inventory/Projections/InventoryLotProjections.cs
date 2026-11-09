using System.Linq.Expressions;

namespace Arb_Int.Domain.Sourcing.Projections
{
    /// <summary>
    /// Innehåller återanvändbara EF-projektioner för inventory lots.
    /// </summary>
    public static class InventoryLotProjections
    {
        /// <summary>
        /// Projekterar en lot till den kompakta listmodellen.
        /// </summary>
        public static Expression<Func<InventoryLot, InventoryLotListResponse>> List => lot => new InventoryLotListResponse
        {
            Id = lot.Id,
            SupplierBusinessRelationshipId = lot.SupplierBusinessRelationshipId,
            SupplierMaterialOfferingId = lot.SupplierMaterialOfferingId,
            MaterialId = lot.MaterialId,
            LotNumber = lot.LotNumber,
            SupplierLotNumber = lot.SupplierLotNumber,
            Name = lot.Name,
            Status = lot.Status,
            TotalQuantity = lot.TotalQuantity,
            AvailableQuantity = lot.AvailableQuantity,
            ReservedQuantity = lot.ReservedQuantity,
            QuantityUnitId = lot.QuantityUnitId,
            BestBeforeUtc = lot.BestBeforeUtc,
            ExpiresAtUtc = lot.ExpiresAtUtc,
            CurrentLocationId = lot.CurrentLocationId,
            RowVersion = lot.RowVersion
        };

        /// <summary>
        /// Projekterar en lot med lot-ägda origins och provenance-länkar.
        /// </summary>
        public static Expression<Func<InventoryLot, InventoryLotResponse>> Details => lot => new InventoryLotResponse
        {
            Id = lot.Id,
            SupplierBusinessRelationshipId = lot.SupplierBusinessRelationshipId,
            SupplierMaterialOfferingId = lot.SupplierMaterialOfferingId,
            MaterialId = lot.MaterialId,
            LotNumber = lot.LotNumber,
            SupplierLotNumber = lot.SupplierLotNumber,
            Name = lot.Name,
            Description = lot.Description,
            Status = lot.Status,
            TotalQuantity = lot.TotalQuantity,
            AvailableQuantity = lot.AvailableQuantity,
            ReservedQuantity = lot.ReservedQuantity,
            QuantityUnitId = lot.QuantityUnitId,
            HarvestedAtUtc = lot.HarvestedAtUtc,
            ProducedAtUtc = lot.ProducedAtUtc,
            BestBeforeUtc = lot.BestBeforeUtc,
            ExpiresAtUtc = lot.ExpiresAtUtc,
            CurrentLocationId = lot.CurrentLocationId,
            WarehouseReference = lot.WarehouseReference,
            InternalNotes = lot.InternalNotes,
            CreateDateUtc = lot.CreateDateUtc,
            Origins = lot.Origins
                .OrderBy(x => x.Role)
                .ThenByDescending(x => x.IsPrimary)
                .ThenBy(x => x.GeographicLocationId)
                .ThenBy(x => x.Id)
                .Select(x => new InventoryLotOriginResponse
                {
                    Id = x.Id,
                    InventoryLotId = x.InventoryLotId,
                    GeographicLocationId = x.GeographicLocationId,
                    Role = x.Role,
                    VerificationStatus = x.VerificationStatus,
                    IsPrimary = x.IsPrimary,
                    Proportion = x.Proportion,
                    Notes = x.Notes,
                    RowVersion = x.RowVersion
                })
                .ToList(),
            ParentLinks = lot.ParentLinks
                .OrderBy(x => x.ParentInventoryLotId)
                .ThenBy(x => x.Kind)
                .ThenBy(x => x.Id)
                .Select(x => new InventoryLotLinkResponse
                {
                    Id = x.Id,
                    ParentInventoryLotId = x.ParentInventoryLotId,
                    ChildInventoryLotId = x.ChildInventoryLotId,
                    Kind = x.Kind,
                    InputQuantity = x.InputQuantity,
                    QuantityUnitId = x.QuantityUnitId,
                    YieldPercent = x.YieldPercent,
                    MaterialTransformationId = x.MaterialTransformationId,
                    Notes = x.Notes,
                    RowVersion = x.RowVersion
                })
                .ToList(),
            ChildLinks = lot.ChildLinks
                .OrderBy(x => x.ChildInventoryLotId)
                .ThenBy(x => x.Kind)
                .ThenBy(x => x.Id)
                .Select(x => new InventoryLotLinkResponse
                {
                    Id = x.Id,
                    ParentInventoryLotId = x.ParentInventoryLotId,
                    ChildInventoryLotId = x.ChildInventoryLotId,
                    Kind = x.Kind,
                    InputQuantity = x.InputQuantity,
                    QuantityUnitId = x.QuantityUnitId,
                    YieldPercent = x.YieldPercent,
                    MaterialTransformationId = x.MaterialTransformationId,
                    Notes = x.Notes,
                    RowVersion = x.RowVersion
                })
                .ToList(),
            RowVersion = lot.RowVersion
        };
    }
}
