using Arb_Int.Domain.Articles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class BuyerRfqRequirementConfig : IEntityTypeConfiguration<BuyerRfqRequirement>
    {
        public void Configure(EntityTypeBuilder<BuyerRfqRequirement> builder)
        {
            builder.ToTable(
                "BuyerRfqRequirements",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_BuyerRfqRequirements_Weight",
                        """
                    "Weight" >= 0
                    """);
                });
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.Level)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.Weight)
                .HasPrecision(9, 6);

            builder.Property(x => x.AlternativeGroupKey)
                .HasMaxLength(100);

            builder.Property(x => x.BuyerOriginalText)
                .HasMaxLength(4096);

            builder.Property(x => x.InternalNotes)
                .HasMaxLength(4096);

            builder.Property<string>("RequirementType")
                .HasMaxLength(32)
                .IsRequired();

            builder.HasIndex(x => new
            {
                x.BuyerRfqRevisionId,
                x.BuyerRfqLineId,
                x.SortOrder
            });

            builder.HasIndex(x => new
            {
                x.Level,
                x.IsActive
            });

            builder.HasOne(x => x.BuyerRfqRevision)
                .WithMany(x => x.Requirements)
                .HasForeignKey(x => x.BuyerRfqRevisionId)
                .OnDelete(DeleteBehavior.Cascade);

            /*
             * Jag använder revisions-id:t i den sammansatta foreign keyn så att
             * ett radkrav aldrig kan peka på en RFQ-rad från en annan revision.
             */
            builder.HasOne(x => x.BuyerRfqLine)
                .WithMany(x => x.Requirements)
                .HasForeignKey(x => new
                {
                    x.BuyerRfqLineId,
                    x.BuyerRfqRevisionId
                })
                .HasPrincipalKey(x => new
                {
                    x.Id,
                    x.BuyerRfqRevisionId
                })
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasDiscriminator<string>("RequirementType")
                .HasValue<BuyerRfqMaterialRequirement>("Material")
                .HasValue<BuyerRfqBiologicalSourceRequirement>("BiologicalSource")
                .HasValue<BuyerRfqOriginRequirement>("Origin")
                .HasValue<BuyerRfqCertificationRequirement>("Certification")
                .HasValue<BuyerRfqDocumentRequirement>("Document")
                .HasValue<BuyerRfqConstituentRequirement>("Constituent")
                .HasValue<BuyerRfqCharacteristicRequirement>("Characteristic")
                .HasValue<BuyerRfqProcessRequirement>("Process")
                .HasValue<BuyerRfqCommercialRequirement>("Commercial")
                .HasValue<BuyerRfqLogisticsRequirement>("Logistics");
        }
    }
}
