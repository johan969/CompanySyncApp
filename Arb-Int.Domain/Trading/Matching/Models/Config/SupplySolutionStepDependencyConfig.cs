using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class SupplySolutionStepDependencyConfig : IEntityTypeConfiguration<SupplySolutionStepDependency>
    {
        public void Configure(EntityTypeBuilder<SupplySolutionStepDependency> builder)
        {
            builder.ToTable(
                "SupplySolutionStepDependencies",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_SupplySolutionStepDependencies_DifferentSteps",
                        """
                    "PredecessorStepId" <> "SuccessorStepId"
                    """);
                });
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ActionName)
                .HasMaxLength(255);
            builder.Property(x => x.RowVersion)
                .IsRowVersion();
            builder.Property(x => x.Notes)
                .HasMaxLength(4096);
            builder.HasIndex(x => new
            {
                x.PredecessorStepId,
                x.SuccessorStepId
            })
                .IsUnique();
            builder.HasOne(x => x.PredecessorStep)
                .WithMany(x => x.SuccessorDependencies)
                .HasForeignKey(x => new
                {
                    x.PredecessorStepId,
                    x.SupplySolutionCandidateId
                })
                .HasPrincipalKey(x => new
                {
                    x.Id,
                    x.SupplySolutionCandidateId
                })
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.SuccessorStep)
                .WithMany(x => x.PredecessorDependencies)
                .HasForeignKey(x => new
                {
                    x.SuccessorStepId,
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
