using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    /// <summary>
    /// Konfigurerar kopplingen mellan BuyerRfq och ScheduledTask.
    /// </summary>
    public sealed class BuyerRfqScheduledTaskLinkConfig : IEntityTypeConfiguration<BuyerRfqScheduledTaskLink>
    {
        /// <summary>
        /// Konfigurerar tabell, relationer och index för RFQ-uppgifter.
        /// </summary>
        /// <param name="builder">EF-konfiguration för länkentiteten.</param>
        public void Configure(EntityTypeBuilder<BuyerRfqScheduledTaskLink> builder)
        {
            builder.ToTable("BuyerRfqScheduledTaskLinks");

            builder.HasKey(x => new { x.BuyerRfqId, x.ScheduledTaskId });

            builder.Property(x => x.Purpose)
                .HasConversion<string>()
                .HasMaxLength(64)
                .IsRequired();

            builder.HasOne(x => x.BuyerRfq)
                .WithMany(x => x.ScheduledTaskLinks)
                .HasForeignKey(x => x.BuyerRfqId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.ScheduledTask)
                .WithMany()
                .HasForeignKey(x => x.ScheduledTaskId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.BuyerRfqId, x.Purpose })
                .HasDatabaseName("IX_BuyerRfqScheduledTaskLinks_Rfq_Purpose");

            builder.HasIndex(x => x.ScheduledTaskId)
                .IsUnique()
                .HasDatabaseName("UX_BuyerRfqScheduledTaskLinks_ScheduledTask");
        }
    }
}