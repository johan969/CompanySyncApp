using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Articles.Configurations
{
    public class GeographicLocationConfig :
        IEntityTypeConfiguration<GeographicLocation>
    {
        public void Configure(EntityTypeBuilder<GeographicLocation> builder)
        {
            builder.ToTable("GeographicLocations", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "CK_GeographicLocations_ScopeCountry",
                    """
                (
                    "Scope" = 'Global'
                    AND "CountryId" IS NULL
                )
                OR
                (
                    "Scope" = 'CountryOrTerritory'
                    AND "CountryId" IS NOT NULL
                )
                """);
            });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Scope)
                .HasConversion<string>()
                .HasMaxLength(32)
                .HasDefaultValue(GeographicScope.CountryOrTerritory);

            builder.Property(x => x.Subregion)
                .HasMaxLength(512);

            builder.Property(x => x.LocationKey)
                .IsRequired()
                .HasMaxLength(1024);

            builder.HasIndex(x => x.LocationKey)
                .IsUnique();

            builder.HasIndex(x => new
            {
                x.CountryId,
                x.WaterId
            });

            builder.HasOne(x => x.Country)
                .WithMany()
                .HasForeignKey(x => x.CountryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Water)
                .WithMany()
                .HasForeignKey(x => x.WaterId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}