using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class MatchingRunLineConfig : IEntityTypeConfiguration<MatchingRunLine>
    {
        public void Configure(EntityTypeBuilder<MatchingRunLine> builder)
        {
            builder.ToTable(
                "MatchingRunLines",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_MatchingRunLines_RequiredQuantity",
                        """
                        "RequiredQuantity" > 0
                        """);
                });

            builder.HasKey(x => x.Id);

            builder.HasAlternateKey(x => new
            {
                x.Id,
                x.MatchingRunId
            });
            builder.Property(x => x.ActionName)
                .HasMaxLength(255);
            builder.Property(x => x.RowVersion)
                .IsRowVersion();
            builder.Property(x => x.RequiredQuantity)
                .HasPrecision(20, 6);
            builder.Property(x => x.InputSnapshotJson)
                .HasColumnType("jsonb");
            builder.HasIndex(x => new
            {
                x.MatchingRunId,
                x.BuyerRfqLineId
            })
                .IsUnique();
            builder.HasOne(x => x.MatchingRun)
                .WithMany(x => x.Lines)
                .HasForeignKey(x => new
                {
                    x.MatchingRunId,
                    x.BuyerRfqRevisionId
                })
                .HasPrincipalKey(x => new
                {
                    x.Id,
                    x.BuyerRfqRevisionId
                })
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.BuyerRfqLine)
                .WithMany()
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
            builder.HasOne(x => x.QuantityUnit)
                .WithMany()
                .HasForeignKey(x => x.QuantityUnitId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
