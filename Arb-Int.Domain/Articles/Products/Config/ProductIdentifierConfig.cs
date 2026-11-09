using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Articles.Configurations
{
    public class ProductIdentifierConfig :
        IEntityTypeConfiguration<ProductIdentifier>
    {
        public void Configure(EntityTypeBuilder<ProductIdentifier> builder)
        {
            builder.ToTable("ProductIdentifiers");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Scheme)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Issuer)
                .HasMaxLength(255);

            builder.Property(x => x.Value)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.IdentifierKey)
                .IsRequired()
                .HasMaxLength(128);

            builder.HasIndex(x => x.IdentifierKey)
                .IsUnique();

            builder.HasIndex(x => x.ProductId);

            /*
             * En produkt får högst ha en primär extern identifierare.
             *
             * Detta är ett partiellt PostgreSQL-index.
             */
            builder.HasIndex(x => x.ProductId)
                .IsUnique()
                .HasFilter("\"IsPrimary\" = TRUE");

            builder.HasOne(x => x.Product)
                .WithMany(x => x.Identifiers)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}