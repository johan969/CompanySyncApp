using Arb_Int.Domain.Articles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class BuyerRfqProcessRequirementConfig : IEntityTypeConfiguration<BuyerRfqProcessRequirement>
    {
        public void Configure(EntityTypeBuilder<BuyerRfqProcessRequirement> builder)
        {
            builder.Property(x => x.Mode)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.ProcessParametersJson)
                .HasColumnType("jsonb");

            builder.HasOne(x => x.ProcessType)
                .WithMany()
                .HasForeignKey(x => x.ProcessTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
