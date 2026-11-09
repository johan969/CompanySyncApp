using Arb_Int.Domain.Articles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class BuyerRfqMaterialRequirementConfig : IEntityTypeConfiguration<BuyerRfqMaterialRequirement>
    {
        public void Configure(EntityTypeBuilder<BuyerRfqMaterialRequirement> builder)
        {
            builder.Property(x => x.MatchMode)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.HasOne(x => x.Material)
                .WithMany()
                .HasForeignKey(x => x.MaterialId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.MaterialType)
                .WithMany()
                .HasForeignKey(x => x.MaterialTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
