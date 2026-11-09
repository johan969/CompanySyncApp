using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class BuyerRfqDocumentRequirementConfig : IEntityTypeConfiguration<BuyerRfqDocumentRequirement>
    {
        public void Configure(EntityTypeBuilder<BuyerRfqDocumentRequirement> builder)
        {
            builder.Property(x => x.Operator)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.HasOne(x => x.DocumentType)
                .WithMany()
                .HasForeignKey(x => x.DocumentTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
