using Arb_Int.Domain.Economy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class SupplierQuoteRevisionConfig : IEntityTypeConfiguration<SupplierQuoteRevision>
    {
        public void Configure(EntityTypeBuilder<SupplierQuoteRevision> builder)
        {
            builder.ToTable(
                "SupplierQuoteRevisions",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_SupplierQuoteRevisions_Validity",
                        """
                        "ValidToUtc" IS NULL
                        OR "ValidFromUtc" IS NULL
                        OR "ValidToUtc" >= "ValidFromUtc"
                        """);

                    table.HasCheckConstraint(
                        "CK_SupplierQuoteRevisions_Snapshot",
                        """
                        (
                            "SnapshotJson" IS NULL
                            AND "SnapshotHash" IS NULL
                        )
                        OR
                        (
                            "SnapshotJson" IS NOT NULL
                            AND "SnapshotHash" IS NOT NULL
                        )
                        """);

                    table.HasCheckConstraint(
                        "CK_SupplierQuoteRevisions_ReceivedSnapshot",
                        """
                        "Status" = 'Draft'
                        OR
                        (
                            "ReceivedAtUtc" IS NOT NULL
                            AND "SnapshotJson" IS NOT NULL
                            AND "SnapshotHash" IS NOT NULL
                        )
                        """);
                });

            builder.HasKey(x => x.Id);

            builder.HasAlternateKey(x => new
            {
                x.Id,
                x.SupplierBusinessRelationshipId
            });
            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.NamedPlace)
                .HasMaxLength(512);

            builder.Property(x => x.PaymentTerms)
                .HasMaxLength(2048);

            builder.Property(x => x.OriginalDocumentReference)
                .HasMaxLength(2048);

            builder.Property(x => x.SnapshotJson)
                .HasColumnType("jsonb");

            builder.Property(x => x.SnapshotHash)
                .HasMaxLength(128);

            builder.Property(x => x.InternalNotes)
                .HasMaxLength(4096);

            builder.HasIndex(x => new
            {
                x.SupplierQuoteId,
                x.RevisionNumber
            })
                .IsUnique();

            /*
             * Bara en redigerbar revision får finnas per quote.
             */
            builder.HasIndex(x => x.SupplierQuoteId)
                .IsUnique()
                .HasFilter("\"Status\" = 'Draft'")
                .HasDatabaseName("UX_SupplierQuoteRevisions_OneDraft");

            /*
             * Received och AcceptedInternally representerar den aktuella
             * kommersiella revisionen. Äldre revisioner ska vara historiska.
             */
            builder.HasIndex(x => x.SupplierQuoteId)
                .IsUnique()
                .HasFilter("\"Status\" IN ('Received', 'AcceptedInternally')")
                .HasDatabaseName("UX_SupplierQuoteRevisions_OneCurrentCommercialRevision");

            builder.HasIndex(x => x.SnapshotHash);

            builder.HasOne(x => x.Currency)
                .WithMany()
                .HasForeignKey(x => x.CurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.IncotermRule)
                .WithMany()
                .HasForeignKey(x => x.IncotermRuleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Lines)
                .WithOne(x => x.SupplierQuoteRevision)
                .HasForeignKey(x => new
                {
                    x.SupplierQuoteRevisionId,
                    x.SupplierBusinessRelationshipId
                })
                .HasPrincipalKey(x => new
                {
                    x.Id,
                    x.SupplierBusinessRelationshipId
                })
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
