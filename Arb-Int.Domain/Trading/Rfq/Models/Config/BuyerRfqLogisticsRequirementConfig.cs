using Arb_Int.Domain.Articles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class BuyerRfqLogisticsRequirementConfig : IEntityTypeConfiguration<BuyerRfqLogisticsRequirement>
    {
        public void Configure(EntityTypeBuilder<BuyerRfqLogisticsRequirement> builder)
        {
            builder.Property(x => x.Kind)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.Operator)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.NumericValue)
                .HasPrecision(24, 10);

            builder.Property(x => x.TextValue)
                .HasMaxLength(2048);

            builder.HasOne(x => x.UnitOfMeasure)
                .WithMany()
                .HasForeignKey(x => x.UnitOfMeasureId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.IncotermRule)
                .WithMany()
                .HasForeignKey(x => x.IncotermRuleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
