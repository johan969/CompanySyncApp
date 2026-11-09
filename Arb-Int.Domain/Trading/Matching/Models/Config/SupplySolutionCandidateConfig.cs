using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class SupplySolutionCandidateConfig : IEntityTypeConfiguration<SupplySolutionCandidate>
    {
        public void Configure(EntityTypeBuilder<SupplySolutionCandidate> builder)
        {
            builder.ToTable(
                "SupplySolutionCandidates",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_SupplySolutionCandidates_Score",
                        """
                        "TotalScore" >= 0
                        """);

                    table.HasCheckConstraint(
                        "CK_SupplySolutionCandidates_Coverage",
                        """
                        "CoveragePercent" >= 0
                        AND "CoveragePercent" <= 100
                        """);

                    table.HasCheckConstraint(
                        "CK_SupplySolutionCandidates_Snapshot",
                        """
                        (
                            "SnapshotJson" IS NULL
                            AND "SnapshotHash" IS NULL
                        )
                        OR
                        (
                            "SnapshotJson" IS NOT NULL
                            AND "SnapshotHash" IS NOT NULL
                        )
                        """);
                });

            builder.HasKey(x => x.Id);

            builder.HasAlternateKey(x => new
            {
                x.Id,
                x.MatchingRunId
            });

            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.Kind)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.Outcome)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.VerificationState)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.TotalScore)
                .HasPrecision(12, 6);

            builder.Property(x => x.CoveragePercent)
                .HasPrecision(9, 4);

            builder.Property(x => x.EstimatedPurchaseCost)
                .HasPrecision(20, 6);

            builder.Property(x => x.EstimatedLogisticsCost)
                .HasPrecision(20, 6);

            builder.Property(x => x.EstimatedProcessingCost)
                .HasPrecision(20, 6);

            builder.Property(x => x.EstimatedTotalCost)
                .HasPrecision(20, 6);

            builder.Property(x => x.SnapshotJson)
                .HasColumnType("jsonb");

            builder.Property(x => x.SnapshotHash)
                .HasMaxLength(128);

            builder.HasIndex(x => new
            {
                x.MatchingRunId,
                x.CandidateNumber
            })
                .IsUnique();

            builder.HasIndex(x => x.SnapshotHash);

            builder.HasIndex(x => new
            {
                x.Status,
                x.Outcome,
                x.TotalScore
            });

            builder.HasIndex(x => new
            {
                x.Status,
                x.ExpiresAtUtc
            })
                .HasFilter("\"ExpiresAtUtc\" IS NOT NULL");

            builder.HasOne(x => x.EstimatedCostCurrency)
                .WithMany()
                .HasForeignKey(x => x.EstimatedCostCurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Steps)
                .WithOne(x => x.SupplySolutionCandidate)
                .HasForeignKey(x => x.SupplySolutionCandidateId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.RequirementEvaluations)
                .WithOne(x => x.SupplySolutionCandidate)
                .HasForeignKey(x => x.SupplySolutionCandidateId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.ScoreContributions)
                .WithOne(x => x.SupplySolutionCandidate)
                .HasForeignKey(x => x.SupplySolutionCandidateId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.ModerationDecisions)
                .WithOne(x => x.SupplySolutionCandidate)
                .HasForeignKey(x => x.SupplySolutionCandidateId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
