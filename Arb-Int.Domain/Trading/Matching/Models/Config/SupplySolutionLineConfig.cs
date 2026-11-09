using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class SupplySolutionLineConfig : IEntityTypeConfiguration<SupplySolutionLine>
    {
        public void Configure(EntityTypeBuilder<SupplySolutionLine> builder)
        {
            builder.ToTable(
                "SupplySolutionLines",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_SupplySolutionLines_Quantities",
                        """
                    "RequiredQuantity" > 0
                    AND "CoveredQuantity" >= 0
                    AND "ShortageQuantity" >= 0
                    AND "CoveredQuantity" + "ShortageQuantity" = "RequiredQuantity"
                    AND "CoveragePercent" >= 0
                    AND "CoveragePercent" <= 100
                    AND (NOT "IsFullyCovered" OR "ShortageQuantity" = 0)
                    """);
                });
            builder.HasKey(x => x.Id);

            builder.HasAlternateKey(x => new
            {
                x.Id,
                x.SupplySolutionCandidateId
            });
            builder.Property(x => x.ActionName)
                .HasMaxLength(255);
            builder.Property(x => x.RowVersion)
                .IsRowVersion();
            builder.Property(x => x.RequiredQuantity)
                .HasPrecision(20, 6);
            builder.Property(x => x.CoveredQuantity)
                .HasPrecision(20, 6);
            builder.Property(x => x.ShortageQuantity)
                .HasPrecision(20, 6);
            builder.Property(x => x.CoveragePercent)
                .HasPrecision(9, 4);
            builder.Property(x => x.Notes)
                .HasMaxLength(4096);
            builder.HasIndex(x => new
            {
                x.SupplySolutionCandidateId,
                x.MatchingRunLineId
            })
                .IsUnique();
            builder.HasOne(x => x.SupplySolutionCandidate)
                .WithMany(x => x.Lines)
                .HasForeignKey(x => new
                {
                    x.SupplySolutionCandidateId,
                    x.MatchingRunId
                })
                .HasPrincipalKey(x => new
                {
                    x.Id,
                    x.MatchingRunId
                })
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.MatchingRunLine)
                .WithMany()
                .HasForeignKey(x => new
                {
                    x.MatchingRunLineId,
                    x.MatchingRunId
                })
                .HasPrincipalKey(x => new
                {
                    x.Id,
                    x.MatchingRunId
                })
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.QuantityUnit)
                .WithMany()
                .HasForeignKey(x => x.QuantityUnitId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(x => x.Allocations)
                .WithOne(x => x.SupplySolutionLine)
                .HasForeignKey(x => x.SupplySolutionLineId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(x => x.CapacityEvaluations)
                .WithOne(x => x.SupplySolutionLine)
                .HasForeignKey(x => x.SupplySolutionLineId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
