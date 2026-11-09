using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class SupplierQuoteConfig : IEntityTypeConfiguration<SupplierQuote>
    {
        public void Configure(EntityTypeBuilder<SupplierQuote> builder)
        {
            builder.ToTable("SupplierQuotes");

            builder.HasKey(x => x.Id);

            builder.HasAlternateKey(x => new
            {
                x.Id,
                x.SupplierBusinessRelationshipId
            });

            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.QuoteNumber)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.SupplierReference)
                .HasMaxLength(255);

            builder.Property(x => x.InternalNotes)
                .HasMaxLength(4096);

            builder.HasIndex(x => x.QuoteNumber)
                .IsUnique();

            builder.HasIndex(x => new
            {
                x.SupplierBusinessRelationshipId,
                x.Status
            });

            builder.HasOne(x => x.SupplierBusinessRelationship)
                .WithMany()
                .HasForeignKey(x => x.SupplierBusinessRelationshipId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.BuyerRfqRevision)
                .WithMany()
                .HasForeignKey(x => x.BuyerRfqRevisionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Revisions)
                .WithOne(x => x.SupplierQuote)
                .HasForeignKey(x => new
                {
                    x.SupplierQuoteId,
                    x.SupplierBusinessRelationshipId
                })
                .HasPrincipalKey(x => new
                {
                    x.Id,
                    x.SupplierBusinessRelationshipId
                })
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
