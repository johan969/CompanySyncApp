using Arb_Int.Domain.Articles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Sourcing.Configurations
{
    public class InventoryLotLinkConfig : IEntityTypeConfiguration<InventoryLotLink>
    {
        public void Configure(EntityTypeBuilder<InventoryLotLink> builder)
        {
            builder.ToTable(
                "InventoryLotLinks",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_InventoryLotLinks_DifferentLots",
                        """
                    "ParentInventoryLotId" <> "ChildInventoryLotId"
                    """);

                    table.HasCheckConstraint(
                        "CK_InventoryLotLinks_Yield",
                        """
                    "YieldPercent" IS NULL
                    OR ("YieldPercent" >= 0 AND "YieldPercent" <= 100)
                    """);
                });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.Kind)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.InputQuantity)
                .HasPrecision(20, 6);

            builder.Property(x => x.YieldPercent)
                .HasPrecision(9, 4);

            builder.Property(x => x.Notes)
                .HasMaxLength(4096);

            builder.HasIndex(x => new
            {
                x.ParentInventoryLotId,
                x.ChildInventoryLotId,
                x.Kind
            })
                .IsUnique();

            builder.HasOne(x => x.ParentInventoryLot)
                .WithMany(x => x.ChildLinks)
                .HasForeignKey(x => x.ParentInventoryLotId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ChildInventoryLot)
                .WithMany(x => x.ParentLinks)
                .HasForeignKey(x => x.ChildInventoryLotId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.QuantityUnit)
                .WithMany()
                .HasForeignKey(x => x.QuantityUnitId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.MaterialTransformation)
                .WithMany()
                .HasForeignKey(x => x.MaterialTransformationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
