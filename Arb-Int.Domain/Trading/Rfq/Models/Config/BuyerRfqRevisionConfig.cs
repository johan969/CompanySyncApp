using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class BuyerRfqRevisionConfig : IEntityTypeConfiguration<BuyerRfqRevision>
    {
        public void Configure(EntityTypeBuilder<BuyerRfqRevision> builder)
        {
            builder.ToTable(
                "BuyerRfqRevisions",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_BuyerRfqRevisions_ReleasedSnapshot",
                        """
                        "Status" NOT IN ('ReleasedForMatching', 'Superseded')
                        OR
                        (
                            "ReleasedAtUtc" IS NOT NULL
                            AND "SnapshotHash" IS NOT NULL
                            AND "SnapshotJson" IS NOT NULL
                        )
                        """);

                    table.HasCheckConstraint(
                        "CK_BuyerRfqRevisions_SupersededDate",
                        """
                        "SupersededAtUtc" IS NULL
                        OR
                        (
                            "ReleasedAtUtc" IS NOT NULL
                            AND "SupersededAtUtc" >= "ReleasedAtUtc"
                        )
                        """);
                });

            builder.HasKey(x => x.Id);

            builder.HasAlternateKey(x => new
            {
                x.Id,
                x.BuyerRfqId
            });

            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.Summary)
                .HasMaxLength(4096);

            builder.Property(x => x.BuyerOriginalText)
                .HasMaxLength(16000);

            builder.Property(x => x.InternalInterpretation)
                .HasMaxLength(16000);

            builder.Property(x => x.SnapshotHash)
                .HasMaxLength(128);

            builder.Property(x => x.SnapshotJson)
                .HasColumnType("jsonb");

            builder.HasIndex(x => new
            {
                x.BuyerRfqId,
                x.RevisionNumber
            })
                .IsUnique();

            builder.HasIndex(x => new
            {
                x.BuyerRfqId,
                x.Status
            });

            builder.HasMany(x => x.Lines)
                .WithOne(x => x.BuyerRfqRevision)
                .HasForeignKey(x => x.BuyerRfqRevisionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Deliveries)
                .WithOne(x => x.BuyerRfqRevision)
                .HasForeignKey(x => x.BuyerRfqRevisionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.BuyerRfqId)
                .IsUnique()
                .HasFilter("\"Status\" = 'Draft'")
                .HasDatabaseName("UX_BuyerRfqRevisions_OneDraftPerRfq");

            builder.HasIndex(x => x.BuyerRfqId)
                .IsUnique()
                .HasFilter("\"Status\" = 'ReleasedForMatching'")
                .HasDatabaseName("UX_BuyerRfqRevisions_OneReleasedPerRfq");
        }
    }
}
