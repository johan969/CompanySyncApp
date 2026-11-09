using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Crm.Configurations
{
    public class BuyerDemandStateHistoryConfig : IEntityTypeConfiguration<BuyerDemandStateHistory>
    {
        public void Configure(EntityTypeBuilder<BuyerDemandStateHistory> builder)
        {
            builder.ToTable(
                "BuyerDemandStateHistory",
                tableBuilder =>
                {
                    tableBuilder.HasCheckConstraint(
                        "CK_BuyerDemandStateHistory_Confidence",
                        """
                    "Confidence" IS NULL
                    OR ("Confidence" >= 0 AND "Confidence" <= 1)
                    """);
                });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.FromKnowledgeLevel)
                .HasConversion<string>()
                .HasMaxLength(32);

            builder.Property(x => x.ToKnowledgeLevel)
                .HasConversion<string>()
                .HasMaxLength(32);

            builder.Property(x => x.FromStatus)
                .HasConversion<string>()
                .HasMaxLength(32);

            builder.Property(x => x.ToStatus)
                .HasConversion<string>()
                .HasMaxLength(32);

            builder.Property(x => x.Confidence)
                .HasPrecision(5, 4);

            builder.Property(x => x.Notes)
                .HasMaxLength(4096);

            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.HasIndex(x => new
            {
                x.BuyerDemandId,
                x.ChangedAtUtc
            });

            builder.HasOne(x => x.BuyerDemand)
                .WithMany(x => x.StateHistory)
                .HasForeignKey(x => x.BuyerDemandId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.BuyerDemandEvidence)
                .WithMany()
                .HasForeignKey(x => new
                {
                    x.BuyerDemandEvidenceId,
                    x.BuyerDemandId
                })
                .HasPrincipalKey(x => new
                {
                    x.Id,
                    x.BuyerDemandId
                })
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
