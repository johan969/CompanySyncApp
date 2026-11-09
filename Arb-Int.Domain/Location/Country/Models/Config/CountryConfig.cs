using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Location.Configurations
{
    public class CountryConfig : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name).HasMaxLength(255);
            builder.Property(c => c.Alpha2Code).HasMaxLength(2);
            builder.Property(c => c.Alpha3Code).HasMaxLength(3);
            builder.Property(c => c.DefaultVAT).HasPrecision(5, 2);
            builder.Property(c => c.DefaultTimeZone).HasMaxLength(255);
            builder.Property(c => c.ISO3166Numeric).HasMaxLength(3);
            builder.Property(c => c.IsEUCountry).HasDefaultValue(false);

            builder.HasOne(c => c.Continent)
                .WithMany(c => c.Countries)
                .HasForeignKey(c => c.ContinentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}