using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Sourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class SupplySolutionStepConfig : IEntityTypeConfiguration<SupplySolutionStep>
    {
        public void Configure(EntityTypeBuilder<SupplySolutionStep> builder)
        {
            builder.ToTable(
                "SupplySolutionSteps",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_SupplySolutionSteps_Yield",
                        """
                    "ExpectedYieldPercent" IS NULL
                    OR ("ExpectedYieldPercent" >= 0 AND "ExpectedYieldPercent" <= 100)
                    """);

                    table.HasCheckConstraint(
                        "CK_SupplySolutionSteps_CapabilityProvider",
                        """
                    "SupplierProcessCapabilityId" IS NULL
                    OR "ProviderBusinessRelationshipId" IS NOT NULL
                    """);

                    table.HasCheckConstraint(
                        "CK_SupplySolutionSteps_Quantities",
                        """
                    (("ExpectedInputQuantity" IS NULL AND "InputQuantityUnitId" IS NULL)
                        OR ("ExpectedInputQuantity" >= 0 AND "InputQuantityUnitId" IS NOT NULL))
                    AND (("ExpectedOutputQuantity" IS NULL AND "OutputQuantityUnitId" IS NULL)
                        OR ("ExpectedOutputQuantity" >= 0 AND "OutputQuantityUnitId" IS NOT NULL))
                    """);

                    table.HasCheckConstraint(
                        "CK_SupplySolutionSteps_PlannedWindow",
                        """
                    "PlannedEndUtc" IS NULL
                    OR "PlannedStartUtc" IS NULL
                    OR "PlannedEndUtc" >= "PlannedStartUtc"
                    """);
                });
            builder.HasKey(x => x.Id);

            builder.HasAlternateKey(x => new
            {
                x.Id,
                x.SupplySolutionCandidateId
            });
            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.Type)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.ExpectedInputQuantity)
                .HasPrecision(20, 6);

            builder.Property(x => x.ExpectedOutputQuantity)
                .HasPrecision(20, 6);

            builder.Property(x => x.ExpectedYieldPercent)
                .HasPrecision(9, 4);

            builder.Property(x => x.ParametersJson)
                .HasColumnType("jsonb");

            builder.HasIndex(x => new
            {
                x.SupplySolutionCandidateId,
                x.SequenceNumber
            })
                .IsUnique();

            builder.HasOne(x => x.SupplySolutionLine)
                .WithMany()
                .HasForeignKey(x => new
                {
                    x.SupplySolutionLineId,
                    x.SupplySolutionCandidateId
                })
                .HasPrincipalKey(x => new
                {
                    x.Id,
                    x.SupplySolutionCandidateId
                })
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ProviderBusinessRelationship)
                .WithMany()
                .HasForeignKey(x => x.ProviderBusinessRelationshipId)
                .OnDelete(DeleteBehavior.Restrict);

            /*
             * Jag binder vald processkapacitet till samma provider-relation som steget.
             */
            builder.HasOne(x => x.SupplierProcessCapability)
                .WithMany()
                .HasForeignKey(x => new
                {
                    x.SupplierProcessCapabilityId,
                    x.ProviderBusinessRelationshipId
                })
                .HasPrincipalKey(x => new
                {
                    x.Id,
                    x.BusinessRelationshipId
                })
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ProcessType)
                .WithMany()
                .HasForeignKey(x => x.ProcessTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.InputMaterial)
                .WithMany()
                .HasForeignKey(x => x.InputMaterialId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.OutputMaterial)
                .WithMany()
                .HasForeignKey(x => x.OutputMaterialId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.InputQuantityUnit)
                .WithMany()
                .HasForeignKey(x => x.InputQuantityUnitId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.OutputQuantityUnit)
                .WithMany()
                .HasForeignKey(x => x.OutputQuantityUnitId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.FromLocation)
                .WithMany()
                .HasForeignKey(x => x.FromLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ToLocation)
                .WithMany()
                .HasForeignKey(x => x.ToLocationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
