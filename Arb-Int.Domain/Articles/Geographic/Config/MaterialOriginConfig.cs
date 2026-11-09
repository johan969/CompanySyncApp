using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Articles.Configurations
{
    public class MaterialOriginConfig :
        IEntityTypeConfiguration<MaterialOrigin>
    {
        public void Configure(EntityTypeBuilder<MaterialOrigin> builder)
        {
            builder.ToTable("MaterialOrigins");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Role)
                .HasConversion<string>()
                .HasMaxLength(32)
                .HasDefaultValue(MaterialOriginRole.Unknown);

            builder.Property(x => x.Notes)
                .HasMaxLength(4096);

            builder.HasIndex(x => new
            {
                x.MaterialId,
                x.GeographicLocationId,
                x.Role
            })
            .IsUnique();

            builder.HasIndex(x => x.GeographicLocationId);

            builder.HasOne(x => x.Material)
                .WithMany(x => x.OriginLinks)
                .HasForeignKey(x => x.MaterialId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.GeographicLocation)
                .WithMany(x => x.MaterialOriginLinks)
                .HasForeignKey(x => x.GeographicLocationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}