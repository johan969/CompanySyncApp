using Arb_Int.Domain.Articles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Crm.Configurations
{
    public class BuyerDemandEvidenceConfig : IEntityTypeConfiguration<BuyerDemandEvidence>
    {
        public void Configure(EntityTypeBuilder<BuyerDemandEvidence> builder)
        {
            builder.ToTable(
                "BuyerDemandEvidence",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_BuyerDemandEvidence_Confidence",
                        """
                    "Confidence" IS NULL
                    OR ("Confidence" >= 0 AND "Confidence" <= 1)
                    """);
                });
            builder.HasKey(x => x.Id);

            builder.HasAlternateKey(x => new
            {
                x.Id,
                x.BuyerDemandId
            });
            builder.Property(x => x.ActionName)
                .HasMaxLength(255);
            builder.Property(x => x.RowVersion)
                .IsRowVersion();
            builder.Property(x => x.SourceKind)
                .HasConversion<string>()
                .HasMaxLength(64);
            builder.Property(x => x.Position)
                .HasConversion<string>()
                .HasMaxLength(64);
            builder.Property(x => x.SourceUri)
                .HasMaxLength(2048);
            builder.Property(x => x.SourceExternalId)
                .HasMaxLength(255);
            builder.Property(x => x.Excerpt)
                .HasMaxLength(4096);
            builder.Property(x => x.Notes)
                .HasMaxLength(4096);
            builder.Property(x => x.Confidence)
                .HasPrecision(9, 6);
            builder.Property(x => x.EvidenceHash)
                .HasMaxLength(128);
            builder.HasIndex(x => new
            {
                x.BuyerDemandId,
                x.ObservedAtUtc
            });

            builder.HasIndex(x => x.EvidenceHash);

            builder.HasOne(x => x.ReferenceEntry)
                .WithMany()
                .HasForeignKey(x => x.ReferenceEntryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.CrmActivity)
                .WithMany()
                .HasForeignKey(x => x.CrmActivityId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
