using Arb_Int.Domain.Articles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class BuyerRfqCharacteristicRequirementConfig : IEntityTypeConfiguration<BuyerRfqCharacteristicRequirement>
    {
        public void Configure(EntityTypeBuilder<BuyerRfqCharacteristicRequirement> builder)
        {
            builder.Property(x => x.Operator)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.ExpectedTextValue)
                .HasMaxLength(2048);

            builder.Property(x => x.ExpectedNumericValue)
                .HasPrecision(24, 10);

            builder.HasOne(x => x.Characteristic)
                .WithMany()
                .HasForeignKey(x => x.CharacteristicId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.UnitOfMeasure)
                .WithMany()
                .HasForeignKey(x => x.UnitOfMeasureId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
