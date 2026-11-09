using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations;

/// <summary>
/// Konfigurerar buyer acceptance, intern approval och kopplingen till exakt proposal-option.
/// </summary>
public sealed class BuyerProposalAcceptanceConfig : IEntityTypeConfiguration<BuyerProposalAcceptance>
{
    /// <summary>
    /// Konfigurerar tabell, constraints, index och relationer för buyer acceptances.
    /// </summary>
    public void Configure(EntityTypeBuilder<BuyerProposalAcceptance> builder)
    {
        builder.ToTable(
            "BuyerProposalAcceptances",
            table =>
            {
                table.HasCheckConstraint(
                    "CK_BuyerProposalAcceptances_Approval",
                    """
                    (
                        "Status" = 'Approved'
                        AND "InternallyApprovedAtUtc" IS NOT NULL
                    )
                    OR
                    (
                        "Status" <> 'Approved'
                        AND "InternallyApprovedAtUtc" IS NULL
                    )
                    """);
            });

        builder.HasKey(x => x.Id);

        builder.HasAlternateKey(x => new
        {
            x.Id,
            x.BuyerProposalOptionId
        });

        builder.Property(x => x.ActionName).HasMaxLength(255);
        builder.Property(x => x.RowVersion).IsRowVersion();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(64);

        builder.Property(x => x.BuyerAcceptanceReference).HasMaxLength(255);
        builder.Property(x => x.InternalApprovalNotes).HasMaxLength(4096);

        builder.HasIndex(x => new
        {
            x.BuyerProposalRevisionId,
            x.BuyerProposalOptionId,
            x.AcceptedAtUtc
        });

        /*
         * Bara en acceptance får vänta på intern approval per revision.
         * Tidigare Approved eller Rejected acceptances ligger kvar historiskt.
         */
        builder.HasIndex(x => x.BuyerProposalRevisionId)
            .IsUnique()
            .HasFilter("\"Status\" = 'PendingInternalApproval'")
            .HasDatabaseName("UX_BuyerProposalAcceptances_OnePendingPerRevision");

        builder.HasIndex(x => new
        {
            x.BuyerProposalOptionId,
            x.Status
        });

        /*
         * Optionen måste tillhöra exakt samma proposal-revision som acceptancen.
         */
        builder.HasOne(x => x.BuyerProposalOption)
            .WithMany()
            .HasForeignKey(x => new
            {
                x.BuyerProposalOptionId,
                x.BuyerProposalRevisionId
            })
            .HasPrincipalKey(x => new
            {
                x.Id,
                x.BuyerProposalRevisionId
            })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Lines)
            .WithOne(x => x.BuyerProposalAcceptance)
            .HasForeignKey(x => new
            {
                x.BuyerProposalAcceptanceId,
                x.BuyerProposalOptionId
            })
            .HasPrincipalKey(x => new
            {
                x.Id,
                x.BuyerProposalOptionId
            })
            .OnDelete(DeleteBehavior.Cascade);
    }
}