using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Location;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class BuyerRfqOriginRequirementConfig : IEntityTypeConfiguration<BuyerRfqOriginRequirement>
    {
        public void Configure(EntityTypeBuilder<BuyerRfqOriginRequirement> builder)
        {
            builder.Property(x => x.OriginRole)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.Operator)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.HasOne(x => x.GeographicLocation)
                .WithMany()
                .HasForeignKey(x => x.GeographicLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Country)
                .WithMany()
                .HasForeignKey(x => x.CountryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
