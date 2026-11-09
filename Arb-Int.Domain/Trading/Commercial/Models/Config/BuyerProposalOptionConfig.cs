using Arb_Int.Domain.Economy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class BuyerProposalOptionConfig : IEntityTypeConfiguration<BuyerProposalOption>
    {
        public void Configure(EntityTypeBuilder<BuyerProposalOption> builder)
        {
            builder.ToTable(
                "BuyerProposalOptions",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_BuyerProposalOptions_Amounts",
                        """
                    "TotalNetAmount" >= 0
                    AND "TotalGrossAmount" >= 0
                    """);
                });
            builder.HasKey(x => x.Id);

            builder.HasAlternateKey(x => new
            {
                x.Id,
                x.BuyerProposalRevisionId
            });
            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.TotalNetAmount)
                .HasPrecision(20, 6);

            builder.Property(x => x.TotalTaxAmount)
                .HasPrecision(20, 6);

            builder.Property(x => x.TotalGrossAmount)
                .HasPrecision(20, 6);

            builder.Property(x => x.BuyerFacingSummary)
                .HasMaxLength(8000);

            builder.Property(x => x.InternalMarginSnapshotJson)
                .HasColumnType("jsonb");

            builder.HasIndex(x => new
            {
                x.BuyerProposalRevisionId,
                x.OptionNumber
            }).IsUnique();

            builder.HasOne(x => x.SupplySolutionCandidate)
                .WithMany()
                .HasForeignKey(x => x.SupplySolutionCandidateId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Currency)
                .WithMany()
                .HasForeignKey(x => x.CurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Lines)
                .WithOne(x => x.BuyerProposalOption)
                .HasForeignKey(x => x.BuyerProposalOptionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
