using Arb_Int.Domain.Articles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class BuyerRfqBiologicalSourceRequirementConfig : IEntityTypeConfiguration<BuyerRfqBiologicalSourceRequirement>
    {
        public void Configure(EntityTypeBuilder<BuyerRfqBiologicalSourceRequirement> builder)
        {
            builder.Property(x => x.Operator)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.HasOne(x => x.BiologicalSource)
                .WithMany()
                .HasForeignKey(x => x.BiologicalSourceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
