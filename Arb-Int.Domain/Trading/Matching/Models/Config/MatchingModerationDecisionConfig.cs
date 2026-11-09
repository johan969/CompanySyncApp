using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class MatchingModerationDecisionConfig : IEntityTypeConfiguration<MatchingModerationDecision>
    {
        public void Configure(EntityTypeBuilder<MatchingModerationDecision> builder)
        {
            builder.ToTable(
                "MatchingModerationDecisions",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_MatchingModerationDecisions_Override",
                        """
                        (
                            NOT "IsManualOverride"
                            AND "OverriddenOutcome" IS NULL
                            AND "OverriddenScore" IS NULL
                        )
                        OR
                        (
                            "IsManualOverride"
                            AND
                            (
                                "OverriddenOutcome" IS NOT NULL
                                OR "OverriddenScore" IS NOT NULL
                            )
                        )
                        """);

                    table.HasCheckConstraint(
                        "CK_MatchingModerationDecisions_OverriddenScore",
                        """
                        "OverriddenScore" IS NULL
                        OR "OverriddenScore" >= 0
                        """);
                });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.Decision)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.OverriddenOutcome)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.OverriddenScore)
                .HasPrecision(12, 6);

            builder.Property(x => x.Reason)
                .HasMaxLength(4096)
                .IsRequired();

            builder.Property(x => x.Notes)
                .HasMaxLength(4096);

            builder.HasIndex(x => new
            {
                x.SupplySolutionCandidateId,
                x.DecidedAtUtc
            });
        }
    }
}
