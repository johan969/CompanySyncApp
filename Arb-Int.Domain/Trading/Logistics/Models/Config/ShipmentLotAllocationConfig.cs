using Arb_Int.Domain.Sourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class ShipmentLotAllocationConfig : IEntityTypeConfiguration<ShipmentLotAllocation>
    {
        public void Configure(EntityTypeBuilder<ShipmentLotAllocation> builder)
        {
            builder.ToTable(
                "ShipmentLotAllocations",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_ShipmentLotAllocations_Quantity",
                        """
                    "AllocatedQuantity" > 0
                    """);
                });
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ActionName)
                .HasMaxLength(255);
            builder.Property(x => x.RowVersion)
                .IsRowVersion();
            builder.Property(x => x.AllocatedQuantity)
                .HasPrecision(20, 6);
            builder.Property(x => x.ContainerReference)
                .HasMaxLength(255);
            builder.Property(x => x.SealReference)
                .HasMaxLength(255);
            builder.HasIndex(x => new
            {
                x.ShipmentId,
                x.InventoryLotId
            });

            builder.HasOne(x => x.ShipmentLeg)
                .WithMany()
                .HasForeignKey(x => new
                {
                    x.ShipmentLegId,
                    x.ShipmentId
                })
                .HasPrincipalKey(x => new
                {
                    x.Id,
                    x.ShipmentId
                })
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.InventoryLot)
                .WithMany()
                .HasForeignKey(x => x.InventoryLotId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.QuantityUnit)
                .WithMany()
                .HasForeignKey(x => x.QuantityUnitId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
