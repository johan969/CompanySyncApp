using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Economy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class BuyerRfqCommercialRequirementConfig : IEntityTypeConfiguration<BuyerRfqCommercialRequirement>
    {
        public void Configure(EntityTypeBuilder<BuyerRfqCommercialRequirement> builder)
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

            builder.HasOne(x => x.Currency)
                .WithMany()
                .HasForeignKey(x => x.CurrencyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
