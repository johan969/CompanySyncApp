using Arb_Int.Domain.Economy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class BuyerProposalRevisionConfig : IEntityTypeConfiguration<BuyerProposalRevision>
    {
        public void Configure(EntityTypeBuilder<BuyerProposalRevision> builder)
        {
            builder.ToTable(
                "BuyerProposalRevisions",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_BuyerProposalRevisions_Validity",
                        """
                        "ValidToUtc" IS NULL
                        OR "ValidFromUtc" IS NULL
                        OR "ValidToUtc" >= "ValidFromUtc"
                        """);

                    table.HasCheckConstraint(
                        "CK_BuyerProposalRevisions_Snapshot",
                        """
                        (
                            "Status" = 'Draft'
                            AND "IssuedAtUtc" IS NULL
                            AND "SnapshotJson" IS NULL
                            AND "SnapshotHash" IS NULL
                        )
                        OR
                        (
                            "Status" <> 'Draft'
                            AND "IssuedAtUtc" IS NOT NULL
                            AND "SnapshotJson" IS NOT NULL
                            AND "SnapshotHash" IS NOT NULL
                        )
                        """);
                });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.CommercialTerms)
                .HasMaxLength(8000);

            builder.Property(x => x.BuyerFacingNotes)
                .HasMaxLength(8000);

            builder.Property(x => x.InternalNotes)
                .HasMaxLength(4096);

            builder.Property(x => x.SnapshotJson)
                .HasColumnType("jsonb");

            builder.Property(x => x.SnapshotHash)
                .HasMaxLength(128);

            builder.HasIndex(x => new
            {
                x.BuyerProposalId,
                x.RevisionNumber
            })
                .IsUnique();

            /*
            * Endast en redigerbar revision får finnas per proposal.
            */
            builder.HasIndex(x => x.BuyerProposalId)
                .IsUnique()
                .HasFilter("\"Status\" = 'Draft'")
                .HasDatabaseName("UX_BuyerProposalRevisions_OneDraft");

            /*
            * Endast en aktuell Issued-revision får finnas samtidigt.
            */
            builder.HasIndex(x => x.BuyerProposalId)
                .IsUnique()
                .HasFilter("\"Status\" = 'Issued'")
                .HasDatabaseName("UX_BuyerProposalRevisions_OneIssued");

            builder.HasIndex(x => x.SnapshotHash);

            builder.HasOne(x => x.BuyerRfqRevision)
                .WithMany()
                .HasForeignKey(x => new
                {
                    x.BuyerRfqRevisionId,
                    x.BuyerRfqId
                })
                .HasPrincipalKey(x => new
                {
                    x.Id,
                    x.BuyerRfqId
                })
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Currency)
                .WithMany()
                .HasForeignKey(x => x.CurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.DisclosurePackageRevision)
                .WithMany()
                .HasForeignKey(x => x.DisclosurePackageRevisionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Options)
                .WithOne(x => x.BuyerProposalRevision)
                .HasForeignKey(x => x.BuyerProposalRevisionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Acceptances)
                .WithOne(x => x.BuyerProposalRevision)
                .HasForeignKey(x => x.BuyerProposalRevisionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
