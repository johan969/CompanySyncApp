using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Sourcing.Configurations
{
    /// <summary>
    /// Konfigurerar geografiska origin-kopplingar för supplier material offerings.
    /// </summary>
    public sealed class SupplierMaterialOfferingOriginConfig : IEntityTypeConfiguration<SupplierMaterialOfferingOrigin>
    {
        /// <summary>
        /// Konfigurerar constraints, index och relationer för offering-origins.
        /// </summary>
        public void Configure(EntityTypeBuilder<SupplierMaterialOfferingOrigin> builder)
        {
            builder.ToTable("SupplierMaterialOfferingOrigins");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Role)
                .HasConversion<string>()
                .HasMaxLength(32)
                .HasDefaultValue(SupplierOfferingOriginRole.Unknown);

            builder.Property(x => x.Notes)
                .HasMaxLength(4096);

            builder.HasIndex(x => new
            {
                x.SupplierMaterialOfferingId,
                x.GeographicLocationId,
                x.Role
            })
            .IsUnique();

            builder.HasIndex(x => new
            {
                x.SupplierMaterialOfferingId,
                x.Role
            })
            .IsUnique()
            .HasFilter("\"IsPrimary\" = TRUE");

            builder.HasIndex(x => x.GeographicLocationId);

            builder.HasOne(x => x.SupplierMaterialOffering)
                .WithMany(x => x.OriginLinks)
                .HasForeignKey(x => x.SupplierMaterialOfferingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.GeographicLocation)
                .WithMany()
                .HasForeignKey(x => x.GeographicLocationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
