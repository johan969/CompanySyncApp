using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class CapacityEvaluationConfig : IEntityTypeConfiguration<CapacityEvaluation>
    {
        public void Configure(EntityTypeBuilder<CapacityEvaluation> builder)
        {
            builder.ToTable("CapacityEvaluations");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.Outcome)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.RequestedQuantity)
                .HasPrecision(20, 6);

            builder.Property(x => x.AvailableQuantity)
                .HasPrecision(20, 6);

            builder.Property(x => x.AllocatedQuantity)
                .HasPrecision(20, 6);

            builder.Property(x => x.Explanation)
                .HasMaxLength(4096);

            builder.HasIndex(x => new
            {
                x.SupplySolutionLineId,
                x.SupplyAllocationId
            });

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

            builder.HasOne(x => x.QuantityUnit)
                .WithMany()
                .HasForeignKey(x => x.QuantityUnitId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
