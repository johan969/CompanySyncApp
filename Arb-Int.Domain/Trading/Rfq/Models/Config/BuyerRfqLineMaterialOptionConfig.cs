using Arb_Int.Domain.Articles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class BuyerRfqLineMaterialOptionConfig : IEntityTypeConfiguration<BuyerRfqLineMaterialOption>
    {
        public void Configure(EntityTypeBuilder<BuyerRfqLineMaterialOption> builder)
        {
            builder.ToTable(
                "BuyerRfqLineMaterialOptions",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_BuyerRfqLineMaterialOptions_Target",
                        """
                    num_nonnulls("MaterialId", "MaterialTypeId", "BiologicalSourceId", "BiologicalSourceClaimId",
                        "BuyerText") >= 1
                    """);
                });
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ActionName)
                .HasMaxLength(255);
            builder.Property(x => x.RowVersion)
                .IsRowVersion();
            builder.Property(x => x.OptionKind)
                .HasConversion<string>()
                .HasMaxLength(64);
            builder.Property(x => x.BuyerText)
                .HasMaxLength(2048);
            builder.Property(x => x.AlternativeGroupKey)
                .HasMaxLength(100);
            builder.Property(x => x.MinimumMappingConfidence)
                .HasPrecision(9, 6);
            builder.HasIndex(x => new
            {
                x.BuyerRfqLineId,
                x.OptionKind,
                x.SortOrder
            });

            builder.HasOne(x => x.Material)
                .WithMany()
                .HasForeignKey(x => x.MaterialId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.MaterialType)
                .WithMany()
                .HasForeignKey(x => x.MaterialTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.BiologicalSource)
                .WithMany()
                .HasForeignKey(x => x.BiologicalSourceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.BiologicalSourceClaim)
                .WithMany()
                .HasForeignKey(x => x.BiologicalSourceClaimId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
