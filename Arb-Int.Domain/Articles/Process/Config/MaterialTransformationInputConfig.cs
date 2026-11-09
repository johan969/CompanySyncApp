using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Articles.Configurations
{
    public class MaterialTransformationInputConfig :
        IEntityTypeConfiguration<MaterialTransformationInput>
    {
        public void Configure(
            EntityTypeBuilder<MaterialTransformationInput> builder)
        {
            builder.ToTable("MaterialTransformationInputs", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "CK_MaterialTransformationInputs_Quantity",
                    "\"Quantity\" IS NULL OR \"Quantity\" >= 0");

                tableBuilder.HasCheckConstraint(
                    "CK_MaterialTransformationInputs_Percentage",
                    "\"Percentage\" IS NULL " +
                    "OR (\"Percentage\" >= 0 AND \"Percentage\" <= 100)");
            });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Role)
                .HasConversion<string>()
                .HasMaxLength(32)
                .HasDefaultValue(TransformationInputRole.Unknown);

            builder.Property(x => x.Quantity)
                .HasPrecision(18, 6);

            builder.Property(x => x.Percentage)
                .HasPrecision(7, 4);

            builder.Property(x => x.Notes)
                .HasMaxLength(4096);

            builder.HasIndex(x => new
            {
                x.MaterialTransformationId,
                x.MaterialId
            })
            .IsUnique();

            builder.HasIndex(x => x.MaterialId);

            builder.HasOne(x => x.MaterialTransformation)
                .WithMany(x => x.Inputs)
                .HasForeignKey(x => x.MaterialTransformationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Material)
                .WithMany(x => x.UsedAsInputIn)
                .HasForeignKey(x => x.MaterialId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.UnitOfMeasure)
                .WithMany(x => x.TransformationInputs)
                .HasForeignKey(x => x.UnitOfMeasureId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}