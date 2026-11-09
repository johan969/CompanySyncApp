using Arb_Int.Domain.Articles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class RequirementEvaluationConfig : IEntityTypeConfiguration<RequirementEvaluation>
    {
        public void Configure(EntityTypeBuilder<RequirementEvaluation> builder)
        {
            builder.ToTable(
                "RequirementEvaluations",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_RequirementEvaluations_AllocationLine",
                        """
                        "SupplyAllocationId" IS NULL
                        OR "SupplySolutionLineId" IS NOT NULL
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

            builder.Property(x => x.Outcome)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.VerificationState)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.ActualNumericValue)
                .HasPrecision(24, 10);

            builder.Property(x => x.ActualTextValue)
                .HasMaxLength(2048);

            builder.Property(x => x.Explanation)
                .HasMaxLength(4096);

            builder.Property(x => x.EvidenceSnapshotJson)
                .HasColumnType("jsonb");

            builder.Property(x => x.RuleTraceJson)
                .HasColumnType("jsonb");

            builder.HasIndex(x => new
            {
                x.SupplySolutionCandidateId,
                x.BuyerRfqRequirementId,
                x.SupplyAllocationId,
                x.SupplySolutionStepId
            });

            builder.HasIndex(x => new
            {
                x.Outcome,
                x.IsHardFail
            });

            builder.HasOne(x => x.BuyerRfqRequirement)
                .WithMany()
                .HasForeignKey(x => x.BuyerRfqRequirementId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.SupplySolutionLine)
                .WithMany()
                .HasForeignKey(x => new
                {
                    x.SupplySolutionLineId,
                    x.SupplySolutionCandidateId
                })
                .HasPrincipalKey(x => new
                {
                    x.Id,
                    x.SupplySolutionCandidateId
                })
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.SupplyAllocation)
                .WithMany()
                .HasForeignKey(x => new
                {
                    x.SupplyAllocationId,
                    x.SupplySolutionLineId
                })
                .HasPrincipalKey(x => new
                {
                    x.Id,
                    x.SupplySolutionLineId
                })
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.SupplySolutionStep)
                .WithMany()
                .HasForeignKey(x => new
                {
                    x.SupplySolutionStepId,
                    x.SupplySolutionCandidateId
                })
                .HasPrincipalKey(x => new
                {
                    x.Id,
                    x.SupplySolutionCandidateId
                })
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.UnitOfMeasure)
                .WithMany()
                .HasForeignKey(x => x.UnitOfMeasureId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
