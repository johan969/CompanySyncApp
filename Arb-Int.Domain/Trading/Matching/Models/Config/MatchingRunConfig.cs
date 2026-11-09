using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class MatchingRunConfig : IEntityTypeConfiguration<MatchingRun>
    {
        public void Configure(EntityTypeBuilder<MatchingRun> builder)
        {
            builder.ToTable(
                "MatchingRuns",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_MatchingRuns_Started",
                        """
                        "StartedAtUtc" IS NULL
                        OR "StartedAtUtc" >= "QueuedAtUtc"
                        """);

                    table.HasCheckConstraint(
                        "CK_MatchingRuns_Completion",
                        """
                        "CompletedAtUtc" IS NULL
                        OR
                        (
                            "StartedAtUtc" IS NOT NULL
                            AND "CompletedAtUtc" >= "StartedAtUtc"
                        )
                        """);

                    table.HasCheckConstraint(
                        "CK_MatchingRuns_StatusTimestamps",
                        """
                        ("Status" = 'Queued' AND "StartedAtUtc" IS NULL AND "CompletedAtUtc" IS NULL)
                        OR
                        ("Status" = 'Running' AND "StartedAtUtc" IS NOT NULL AND "CompletedAtUtc" IS NULL)
                        OR
                        ("Status" IN ('Completed', 'Failed') AND "StartedAtUtc" IS NOT NULL AND "CompletedAtUtc" IS NOT NULL)
                        OR
                        ("Status" = 'Cancelled' AND "CompletedAtUtc" IS NOT NULL)
                        """);

                    table.HasCheckConstraint(
                        "CK_MatchingRuns_InputSnapshot",
                        """
                        "InputSnapshotJson" IS NOT NULL
                        AND "InputSnapshotHash" IS NOT NULL
                        """);
                });

            builder.HasKey(x => x.Id);

            builder.HasAlternateKey(x => new
            {
                x.Id,
                x.BuyerRfqRevisionId
            });

            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.TriggerName)
                .HasMaxLength(255);

            builder.Property(x => x.CorrelationId)
                .HasMaxLength(255);

            builder.Property(x => x.InputSnapshotJson)
                .HasColumnType("jsonb");

            builder.Property(x => x.InputSnapshotHash)
                .HasMaxLength(128);

            builder.Property(x => x.ErrorMessage)
                .HasMaxLength(8000);

            builder.HasIndex(x => new
            {
                x.BuyerRfqRevisionId,
                x.QueuedAtUtc
            });

            builder.HasIndex(x => new
            {
                x.Status,
                x.QueuedAtUtc
            });

            builder.HasIndex(x => new
            {
                x.Status,
                x.StartedAtUtc
            });

            builder.HasOne(x => x.BuyerRfqRevision)
                .WithMany(x => x.MatchingRuns)
                .HasForeignKey(x => x.BuyerRfqRevisionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.MatchingRuleSetVersion)
                .WithMany(x => x.MatchingRuns)
                .HasForeignKey(x => x.MatchingRuleSetVersionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Candidates)
                .WithOne(x => x.MatchingRun)
                .HasForeignKey(x => x.MatchingRunId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
