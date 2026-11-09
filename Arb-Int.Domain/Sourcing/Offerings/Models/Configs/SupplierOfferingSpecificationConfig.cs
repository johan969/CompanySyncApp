using Arb_Int.Domain.Articles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Sourcing.Configurations
{
    public class SupplierOfferingSpecificationConfig : IEntityTypeConfiguration<SupplierOfferingSpecification>
    {
        public void Configure(EntityTypeBuilder<SupplierOfferingSpecification> builder)
        {
            builder.ToTable(
                "SupplierOfferingSpecifications",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_SupplierOfferingSpecifications_Target",
                        """
                    ("Kind" <> 'Constituent' OR "ConstituentId" IS NOT NULL)
                    AND ("Kind" <> 'Characteristic' OR "CharacteristicId" IS NOT NULL)
                    """);

                    table.HasCheckConstraint(
                        "CK_SupplierOfferingSpecifications_Range",
                        """
                    "MinimumValue" IS NULL
                    OR "MaximumValue" IS NULL
                    OR "MaximumValue" >= "MinimumValue"
                    """);

                    table.HasCheckConstraint(
                        "CK_SupplierOfferingSpecifications_Validity",
                        """
                    "ValidToUtc" IS NULL
                    OR "ValidFromUtc" IS NULL
                    OR "ValidToUtc" >= "ValidFromUtc"
                    """);
                });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.Kind)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.Description)
                .IsRequired();

            builder.Property(x => x.Operator)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.MinimumValue)
                .HasPrecision(24, 10);

            builder.Property(x => x.MaximumValue)
                .HasPrecision(24, 10);

            builder.Property(x => x.TargetValue)
                .HasPrecision(24, 10);

            builder.Property(x => x.TextValue)
                .HasMaxLength(2048);

            builder.Property(x => x.Basis)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.AnalysisMethod)
                .HasMaxLength(1000);

            builder.Property(x => x.VerificationState)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.HasIndex(x => new
            {
                x.SupplierMaterialOfferingId,
                x.Kind,
                x.ValidFromUtc
            });

            builder.HasOne(x => x.SupplierMaterialOffering)
                .WithMany(x => x.Specifications)
                .HasForeignKey(x => x.SupplierMaterialOfferingId)
                .OnDelete(DeleteBehavior.Restrict);

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

            builder.HasOne(x => x.SupplyDocument)
                .WithMany()
                .HasForeignKey(x => x.SupplyDocumentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
