using Arb_Int.Domain.Articles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Sourcing.Configurations
{
    public class SupplyAnalysisResultConfig : IEntityTypeConfiguration<SupplyAnalysisResult>
    {
        public void Configure(EntityTypeBuilder<SupplyAnalysisResult> builder)
        {
            builder.ToTable(
                "SupplyAnalysisResults",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_SupplyAnalysisResults_Analyte",
                        """
                        num_nonnulls("ConstituentId", "CharacteristicId") <= 1
                        AND
                        num_nonnulls("ConstituentId", "CharacteristicId", "AnalyteName") >= 1
                        """);
                });
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ActionName)
                .HasMaxLength(255);
            builder.Property(x => x.RowVersion)
                .IsRowVersion();
            builder.Property(x => x.AnalyteName)
                .HasMaxLength(255);
            builder.Property(x => x.Qualifier)
                .HasConversion<string>()
                .HasMaxLength(64);
            builder.Property(x => x.NumericValue)
                .HasPrecision(24, 10);
            builder.Property(x => x.RangeMaximumValue)
                .HasPrecision(24, 10);
            builder.Property(x => x.TextValue)
                .HasMaxLength(2048);
            builder.Property(x => x.Basis)
                .HasConversion<string>()
                .HasMaxLength(64);
            builder.Property(x => x.DetectionLimit)
                .HasPrecision(24, 10);
            builder.Property(x => x.AnalysisMethod)
                .HasMaxLength(1000);
            builder.Property(x => x.VerificationState)
                .HasConversion<string>()
                .HasMaxLength(64);
            builder.Property(x => x.Notes)
                .HasMaxLength(4096);
            builder.HasIndex(x => new
            {
                x.SupplyAnalysisId,
                x.ConstituentId
            });

            builder.HasOne(x => x.Constituent)
                .WithMany()
                .HasForeignKey(x => x.ConstituentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Characteristic)
                .WithMany()
                .HasForeignKey(x => x.CharacteristicId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.UnitOfMeasure)
                .WithMany()
                .HasForeignKey(x => x.UnitOfMeasureId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
