using Arb_Int.Domain.Crm;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class BuyerRfqConfig : IEntityTypeConfiguration<BuyerRfq>
    {
        public void Configure(EntityTypeBuilder<BuyerRfq> builder)
        {
            builder.ToTable("BuyerRfqs");

            builder.HasKey(x => x.Id);

            builder.HasAlternateKey(x => new
            {
                x.Id,
                x.BuyerBusinessRelationshipId
            });

            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.RfqNumber)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.Priority)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.Visibility)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.BuyerReference)
                .HasMaxLength(255);

            builder.Property(x => x.InternalNotes)
                .HasMaxLength(4096);

            builder.HasIndex(x => x.RfqNumber)
                .IsUnique();

            builder.HasIndex(x => new
            {
                x.BuyerBusinessRelationshipId,
                x.Status
            });

            builder.HasIndex(x => new
            {
                x.OwnerUserId,
                x.Status
            });

            builder.HasOne(x => x.BuyerBusinessRelationship)
                .WithMany()
                .HasForeignKey(x => x.BuyerBusinessRelationshipId)
                .OnDelete(DeleteBehavior.Restrict);

            /*
             * Jag tar med buyer-relationen så att RFQ:n inte kan kopplas till
             * en demand som tillhör en annan köpare.
             */
            builder.HasOne(x => x.BuyerDemand)
                .WithMany()
                .HasForeignKey(x => new
                {
                    x.BuyerDemandId,
                    x.BuyerBusinessRelationshipId
                })
                .HasPrincipalKey(x => new
                {
                    x.Id,
                    x.BusinessRelationshipId
                })
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Revisions)
                .WithOne(x => x.BuyerRfq)
                .HasForeignKey(x => x.BuyerRfqId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
