using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class BuyerProposalAcceptanceLineConfig : IEntityTypeConfiguration<BuyerProposalAcceptanceLine>
    {
        public void Configure(EntityTypeBuilder<BuyerProposalAcceptanceLine> builder)
        {
            builder.ToTable(
                "BuyerProposalAcceptanceLines",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_BuyerProposalAcceptanceLines_Quantity",
                        """
                    "AcceptedQuantity" > 0
                    """);
                });
            builder.HasKey(x => x.Id);

            builder.HasAlternateKey(x => new
            {
                x.Id,
                x.BuyerProposalAcceptanceId
            });
            builder.Property(x => x.ActionName)
                .HasMaxLength(255);
            builder.Property(x => x.RowVersion)
                .IsRowVersion();
            builder.Property(x => x.AcceptedQuantity)
                .HasPrecision(20, 6);
            builder.Property(x => x.BuyerNotes)
                .HasMaxLength(4096);
            builder.Property(x => x.InternalNotes)
                .HasMaxLength(4096);
            builder.HasIndex(x => new
            {
                x.BuyerProposalAcceptanceId,
                x.BuyerProposalLineId
            })
                .IsUnique();
            builder.HasOne(x => x.BuyerProposalLine)
                .WithMany()
                .HasForeignKey(x => new
                {
                    x.BuyerProposalLineId,
                    x.BuyerProposalOptionId
                })
                .HasPrincipalKey(x => new
                {
                    x.Id,
                    x.BuyerProposalOptionId
                })
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.QuantityUnit)
                .WithMany()
                .HasForeignKey(x => x.QuantityUnitId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
