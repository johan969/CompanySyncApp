using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Articles.Configurations
{
    public class UnitOfMeasureConfig :
        IEntityTypeConfiguration<UnitOfMeasure>
    {
        public void Configure(EntityTypeBuilder<UnitOfMeasure> builder)
        {
            builder.ToTable("UnitsOfMeasure");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Symbol)
                .IsRequired()
                .HasMaxLength(32);

            builder.Property(x => x.Dimension)
                .HasConversion<string>()
                .HasDefaultValue(UnitDimension.Mass);

            builder.Property(x => x.FactorToBaseUnit)
                .IsRequired();

            builder.Property(x => x.IsBaseUnit)
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(x => x.SortOrder)
                .IsRequired();

            builder.HasIndex(x => x.Code)
                .IsUnique();

            builder.Property(x => x.IsActive)
                .HasDefaultValue(true)
                .IsRequired();

            builder.HasIndex(x => new { x.Code, x.Dimension, x.SortOrder })
                .HasFilter("\"IsActive\" = TRUE")
                .IsUnique();
        }
    }
}