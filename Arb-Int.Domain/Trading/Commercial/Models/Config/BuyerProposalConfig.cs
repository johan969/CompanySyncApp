using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class BuyerProposalConfig : IEntityTypeConfiguration<BuyerProposal>
    {
        public void Configure(EntityTypeBuilder<BuyerProposal> builder)
        {
            builder.ToTable("BuyerProposals");

            builder.HasKey(x => x.Id);

            builder.HasAlternateKey(x => new
            {
                x.Id,
                x.BuyerRfqId
            });

            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.ProposalNumber)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.InternalNotes)
                .HasMaxLength(4096);

            builder.HasIndex(x => x.ProposalNumber)
                .IsUnique();

            builder.HasIndex(x => new
            {
                x.BuyerBusinessRelationshipId,
                x.Status
            });

            builder.HasOne(x => x.BuyerBusinessRelationship)
                .WithMany()
                .HasForeignKey(x => x.BuyerBusinessRelationshipId)
                .OnDelete(DeleteBehavior.Restrict);

            /*
             * Jag binder förslaget till en RFQ för samma buyer-relation.
             */
            builder.HasOne(x => x.BuyerRfq)
                .WithMany()
                .HasForeignKey(x => new
                {
                    x.BuyerRfqId,
                    x.BuyerBusinessRelationshipId
                })
                .HasPrincipalKey(x => new
                {
                    x.Id,
                    x.BuyerBusinessRelationshipId
                })
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Revisions)
                .WithOne(x => x.BuyerProposal)
                .HasForeignKey(x => new
                {
                    x.BuyerProposalId,
                    x.BuyerRfqId
                })
                .HasPrincipalKey(x => new
                {
                    x.Id,
                    x.BuyerRfqId
                })
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
