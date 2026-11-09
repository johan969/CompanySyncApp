using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class BuyerRfqDeliveryLineAllocationConfig : IEntityTypeConfiguration<BuyerRfqDeliveryLineAllocation>
    {
        public void Configure(EntityTypeBuilder<BuyerRfqDeliveryLineAllocation> builder)
        {
            builder.ToTable(
                "BuyerRfqDeliveryLineAllocations",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_BuyerRfqDeliveryLineAllocations_Quantity",
                        """
                    "Quantity" > 0
                    """);
                });
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ActionName)
                .HasMaxLength(255);
            builder.Property(x => x.RowVersion)
                .IsRowVersion();
            builder.Property(x => x.Quantity)
                .HasPrecision(20, 6);
            builder.HasIndex(x => new
            {
                x.BuyerRfqDeliveryId,
                x.BuyerRfqLineId
            })
                .IsUnique();
            builder.HasOne(x => x.BuyerRfqDelivery)
                .WithMany(x => x.LineAllocations)
                .HasForeignKey(x => new
                {
                    x.BuyerRfqDeliveryId,
                    x.BuyerRfqRevisionId
                })
                .HasPrincipalKey(x => new
                {
                    x.Id,
                    x.BuyerRfqRevisionId
                })
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.BuyerRfqLine)
                .WithMany(x => x.DeliveryAllocations)
                .HasForeignKey(x => new
                {
                    x.BuyerRfqLineId,
                    x.BuyerRfqRevisionId
                })
                .HasPrincipalKey(x => new
                {
                    x.Id,
                    x.BuyerRfqRevisionId
                })
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.QuantityUnit)
                .WithMany()
                .HasForeignKey(x => x.QuantityUnitId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
