using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class ExternalConversationConfig : IEntityTypeConfiguration<ExternalConversation>
    {
        public void Configure(EntityTypeBuilder<ExternalConversation> builder)
        {
            builder.ToTable("ExternalConversations");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.PrimaryChannel)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.Subject)
                .HasMaxLength(500);

            builder.Property(x => x.InternalNotes)
                .HasMaxLength(4096);

            builder.HasIndex(x => new
            {
                x.ExternalBusinessRelationshipId,
                x.Status,
                x.LastMessageAtUtc
            });

            builder.HasOne(x => x.ExternalBusinessRelationship)
                .WithMany()
                .HasForeignKey(x => x.ExternalBusinessRelationshipId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.BuyerRfq)
                .WithMany()
                .HasForeignKey(x => x.BuyerRfqId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.SupplierQuote)
                .WithMany()
                .HasForeignKey(x => x.SupplierQuoteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.BuyerProposal)
                .WithMany()
                .HasForeignKey(x => x.BuyerProposalId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.BuyerSalesOrder)
                .WithMany()
                .HasForeignKey(x => x.BuyerSalesOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.SupplierPurchaseOrder)
                .WithMany()
                .HasForeignKey(x => x.SupplierPurchaseOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Messages)
                .WithOne(x => x.ExternalConversation)
                .HasForeignKey(x => x.ExternalConversationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
