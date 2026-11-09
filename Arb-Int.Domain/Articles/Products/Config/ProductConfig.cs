using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Articles.Configurations
{
    public class ProductConfig : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "CK_Products_SalesUnitQuantity_Positive",
                    "\"SalesUnitQuantity\" > 0");

                /*
                 * Nettoinnehåll och dess enhet ska antingen båda vara null,
                 * eller båda vara angivna.
                 */
                tableBuilder.HasCheckConstraint(
                    "CK_Products_NetContent_Complete",
                    """
                (
                    "NetContentQuantity" IS NULL
                    AND "NetContentUnitOfMeasureId" IS NULL
                )
                OR
                (
                    "NetContentQuantity" IS NOT NULL
                    AND "NetContentQuantity" > 0
                    AND "NetContentUnitOfMeasureId" IS NOT NULL
                )
                """);

                tableBuilder.HasCheckConstraint(
                    "CK_Products_DiscontinuedDateUtc",
                    """
                "DiscontinuedDateUtc" IS NULL
                OR "AvailableFromUtc" IS NULL
                OR "DiscontinuedDateUtc" >= "AvailableFromUtc"
                """);
            });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Sku)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.NormalizedSku)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Slug)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.ShortDescription)
                .HasMaxLength(1024);

            builder.Property(x => x.Notes)
                .HasMaxLength(4096);

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(32)
                .HasDefaultValue(ProductStatus.Draft);

            builder.Property(x => x.SalesUnitQuantity)
                .HasPrecision(18, 6)
                .HasDefaultValue(1m);

            builder.Property(x => x.NetContentQuantity)
                .HasPrecision(18, 6);

            /*
             * NormalizedSku är den egentliga unika jämförelsenyckeln.
             * Det förhindrar exempelvis att både:
             *
             * ALG-ALA-250
             * alg-ala-250
             *
             * registreras som olika produkter.
             */
            builder.HasIndex(x => x.NormalizedSku)
                .IsUnique();

            builder.HasIndex(x => x.Sku);

            builder.HasIndex(x => x.Slug)
                .IsUnique();

            builder.HasIndex(x => x.MaterialId);

            builder.HasIndex(x => new
            {
                x.Status,
                x.AvailableFromUtc
            });

            builder.HasOne(x => x.Material)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.MaterialId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.SalesUnitOfMeasure)
                .WithMany()
                .HasForeignKey(x => x.SalesUnitOfMeasureId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.NetContentUnitOfMeasure)
                .WithMany()
                .HasForeignKey(x => x.NetContentUnitOfMeasureId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}