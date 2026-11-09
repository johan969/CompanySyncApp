using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    /// <summary>
    /// Konfigurerar kopplingen mellan BuyerProposalRevision och ScheduledTask.
    /// </summary>
    public sealed class BuyerProposalRevisionScheduledTaskLinkConfig : IEntityTypeConfiguration<BuyerProposalRevisionScheduledTaskLink>
    {
        /// <summary>
        /// Konfigurerar tabell, relationer och index för proposal-revisionens uppgifter.
        /// </summary>
        /// <param name="builder">EF-konfiguration för länkentiteten.</param>
        public void Configure(EntityTypeBuilder<BuyerProposalRevisionScheduledTaskLink> builder)
        {
            builder.ToTable("BuyerProposalRevisionScheduledTaskLinks");

            builder.HasKey(x => new { x.BuyerProposalRevisionId, x.ScheduledTaskId });

            builder.Property(x => x.Purpose)
                .HasConversion<string>()
                .HasMaxLength(64)
                .IsRequired();

            builder.HasOne(x => x.BuyerProposalRevision)
                .WithMany(x => x.ScheduledTaskLinks)
                .HasForeignKey(x => x.BuyerProposalRevisionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.ScheduledTask)
                .WithMany()
                .HasForeignKey(x => x.ScheduledTaskId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.BuyerProposalRevisionId, x.Purpose })
                .HasDatabaseName("IX_BuyerProposalRevisionScheduledTaskLinks_Revision_Purpose");

            builder.HasIndex(x => x.ScheduledTaskId)
                .IsUnique()
                .HasDatabaseName("UX_BuyerProposalRevisionScheduledTaskLinks_ScheduledTask");
        }
    }
}