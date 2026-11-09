using Arb_Int.Domain.Articles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class BuyerRfqConstituentRequirementConfig : IEntityTypeConfiguration<BuyerRfqConstituentRequirement>
    {
        public void Configure(EntityTypeBuilder<BuyerRfqConstituentRequirement> builder)
        {
            builder.Property(x => x.Operator)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.MinimumValue)
                .HasPrecision(24, 10);

            builder.Property(x => x.MaximumValue)
                .HasPrecision(24, 10);

            builder.Property(x => x.TargetValue)
                .HasPrecision(24, 10);

            builder.Property(x => x.Basis)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.AnalysisMethod)
                .HasMaxLength(1000);

            builder.HasOne(x => x.Constituent)
                .WithMany()
                .HasForeignKey(x => x.ConstituentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.UnitOfMeasure)
                .WithMany()
                .HasForeignKey(x => x.UnitOfMeasureId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
