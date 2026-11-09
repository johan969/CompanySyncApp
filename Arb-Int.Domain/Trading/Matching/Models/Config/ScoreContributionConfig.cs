using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class ScoreContributionConfig : IEntityTypeConfiguration<ScoreContribution>
    {
        public void Configure(EntityTypeBuilder<ScoreContribution> builder)
        {
            builder.ToTable("ScoreContributions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.Category)
                .HasMaxLength(255);

            builder.Property(x => x.Weight)
                .HasPrecision(12, 6);

            builder.Property(x => x.NormalizedScore)
                .HasPrecision(12, 6);

            builder.Property(x => x.Points)
                .HasPrecision(12, 6);

            builder.Property(x => x.Explanation)
                .HasMaxLength(4096);

            builder.HasIndex(x => new
            {
                x.SupplySolutionCandidateId,
                x.Category
            });

            builder.HasOne(x => x.RequirementEvaluation)
                .WithMany()
                .HasForeignKey(x => new
                {
                    x.RequirementEvaluationId,
                    x.SupplySolutionCandidateId
                })
                .HasPrincipalKey(x => new
                {
                    x.Id,
                    x.SupplySolutionCandidateId
                })
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
