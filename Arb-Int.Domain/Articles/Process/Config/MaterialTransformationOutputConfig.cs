using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Articles.Configurations
{
    public class MaterialTransformationOutputConfig :
        IEntityTypeConfiguration<MaterialTransformationOutput>
    {
        public void Configure(
            EntityTypeBuilder<MaterialTransformationOutput> builder)
        {
            builder.ToTable("MaterialTransformationOutputs", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "CK_MaterialTransformationOutputs_Quantity",
                    "\"Quantity\" IS NULL OR \"Quantity\" >= 0");

                tableBuilder.HasCheckConstraint(
                    "CK_MaterialTransformationOutputs_Yield",
                    "\"YieldPercent\" IS NULL " +
                    "OR (\"YieldPercent\" >= 0 AND \"YieldPercent\" <= 100)");
            });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Role)
                .HasConversion<string>()
                .HasMaxLength(32)
                .HasDefaultValue(TransformationOutputRole.Unknown);

            builder.Property(x => x.Quantity)
                .HasPrecision(18, 6);

            builder.Property(x => x.YieldPercent)
                .HasPrecision(7, 4);

            builder.Property(x => x.Notes)
                .HasMaxLength(4096);

            builder.HasIndex(x => new
            {
                x.MaterialTransformationId,
                x.MaterialId
            })
            .IsUnique();

            /*
             * Ett material får i denna modell endast ha en producerande
             * transformation.
             *
             * Om samma typ av material kan framställas genom två olika
             * processvägar skapas två separata materialvarianter.
             */
            builder.HasIndex(x => x.MaterialId)
                .IsUnique();

            builder.HasOne(x => x.MaterialTransformation)
                .WithMany(x => x.Outputs)
                .HasForeignKey(x => x.MaterialTransformationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Material)
                .WithOne(x => x.ProductionOutput)
                .HasForeignKey<MaterialTransformationOutput>(
                    x => x.MaterialId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.UnitOfMeasure)
                .WithMany(x => x.TransformationOutputs)
                .HasForeignKey(x => x.UnitOfMeasureId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}