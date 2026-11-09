using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Articles.Configurations
{
    public class MaterialBiologicalSourceConfig :
        IEntityTypeConfiguration<MaterialBiologicalSource>
    {
        public void Configure(
            EntityTypeBuilder<MaterialBiologicalSource> builder)
        {
            builder.ToTable("MaterialBiologicalSources", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "CK_MaterialBiologicalSources_Proportion",
                    "\"ProportionPercent\" IS NULL " +
                    "OR (\"ProportionPercent\" >= 0 " +
                    "AND \"ProportionPercent\" <= 100)");
            });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Role)
                .HasConversion<string>()
                .HasMaxLength(32)
                .HasDefaultValue(MaterialBiologicalSourceRole.Unknown);

            builder.Property(x => x.ProportionPercent)
                .HasPrecision(7, 4);

            builder.Property(x => x.Notes)
                .HasMaxLength(4096);

            builder.HasIndex(x => new
            {
                x.MaterialId,
                x.BiologicalSourceId
            })
            .IsUnique();

            builder.HasIndex(x => x.BiologicalSourceId);

            builder.HasOne(x => x.Material)
                .WithMany(x => x.BiologicalSourceLinks)
                .HasForeignKey(x => x.MaterialId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.BiologicalSource)
                .WithMany(x => x.MaterialLinks)
                .HasForeignKey(x => x.BiologicalSourceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}