using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Crm.Configurations
{
    /// <summary>
    /// Konfigurerar kopplingen mellan CrmActivity och ScheduledTask.
    /// </summary>
    public sealed class CrmActivityScheduledTaskLinkConfig : IEntityTypeConfiguration<CrmActivityScheduledTaskLink>
    {
        /// <summary>
        /// Konfigurerar tabell, relationer och index för CRM-aktivitetens uppgifter.
        /// </summary>
        /// <param name="builder">EF-konfiguration för länkentiteten.</param>
        public void Configure(EntityTypeBuilder<CrmActivityScheduledTaskLink> builder)
        {
            builder.ToTable("CrmActivityScheduledTaskLinks");

            builder.HasKey(x => new { x.CrmActivityId, x.ScheduledTaskId });

            builder.Property(x => x.Purpose)
                .HasConversion<string>()
                .HasMaxLength(64)
                .IsRequired();

            builder.HasOne(x => x.CrmActivity)
                .WithMany(x => x.ScheduledTaskLinks)
                .HasForeignKey(x => x.CrmActivityId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.ScheduledTask)
                .WithMany()
                .HasForeignKey(x => x.ScheduledTaskId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.CrmActivityId, x.Purpose })
                .HasDatabaseName("IX_CrmActivityScheduledTaskLinks_Activity_Purpose");

            builder.HasIndex(x => x.ScheduledTaskId)
                .IsUnique()
                .HasDatabaseName("UX_CrmActivityScheduledTaskLinks_ScheduledTask");
        }
    }
}