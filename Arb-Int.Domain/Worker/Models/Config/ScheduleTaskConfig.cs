
using Arb_Int.Domain.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Worker.Configurations
{
    public class ScheduleTaskConfig : IEntityTypeConfiguration<ScheduledTask>
    {
        public void Configure(EntityTypeBuilder<ScheduledTask> builder)
        {
            builder.ToTable("ScheduledTasks");

            builder.HasKey(st => st.Id);

            builder.HasOne(st => st.User)
                .WithMany(us => us.ScheduledTasks)
                .HasForeignKey(st => st.UserId)
                .OnDelete(DeleteBehavior.ClientCascade);

            builder.HasOne(st => st.Party)
                .WithMany(n => n.ScheduledTaskLinks)
                .HasForeignKey(st => st.PartyId)
                .OnDelete(DeleteBehavior.ClientCascade);


            builder.HasOne(st => st.ContactPoint)
                .WithMany(n => n.ScheduledTaskLinks)
                .HasForeignKey(st => st.ContactPointId)
                .OnDelete(DeleteBehavior.ClientCascade);

            builder.HasOne(st => st.Definition)
                .WithMany(d => d.Occurrences)
                .HasForeignKey(st => st.DefinitionId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Property(e => e.TaskType).HasConversion<string>().HasMaxLength(64);
            builder.Property(e => e.TriggerType).HasConversion<string>().HasMaxLength(64);
            builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(32);
            builder.Property(e => e.Priority).HasConversion<string>().HasMaxLength(16);

            builder.Property(e => e.RunAtUtc).IsRequired();

            builder.Property(e => e.LockedBy).HasMaxLength(128);
            builder.Property(e => e.HangfireJobId).HasMaxLength(64);
            builder.Property(e => e.IdempotencyKey).HasMaxLength(256);

            builder.Property(e => e.PayloadJson).HasColumnType("text");
            builder.Property(e => e.LastError).HasColumnType("text");

            builder.Property(x => x.RowVersion).IsRowVersion();

            // claim index
            builder.HasIndex(e => new { e.Status, e.RunAtUtc, e.Id })
                .HasDatabaseName("IX_ScheduledTasks_Claim");

            builder.HasIndex(e => e.HangfireJobId)
                .HasDatabaseName("IX_ScheduledTasks_HangfireJobId");

            builder.HasIndex(e => e.LockedUntilUtc)
                .HasDatabaseName("IX_ScheduledTasks_LockedUntilUtc");

            builder.HasIndex(e => new { e.PartyId, e.Status, e.RunAtUtc })
                .HasDatabaseName("IX_ScheduledTasks_Organisation_Status_RunAtUtc");

            builder.HasIndex(e => new { e.ContactPointId, e.Status, e.RunAtUtc })
                .HasDatabaseName("IX_ScheduledTasks_Contact_Status_RunAtUtc");

            builder.HasIndex(e => e.DefinitionId)
                .HasDatabaseName("IX_ScheduledTasks_DefinitionId");

            // one occurrence per definition+runAt
            builder.HasIndex(e => new { e.DefinitionId, e.IdempotencyKey })
                .HasDatabaseName("UX_ScheduledTasks_Definition_Occurrence")
                .IsUnique()
                .HasFilter("\"DefinitionId\" IS NOT NULL AND \"IdempotencyKey\" IS NOT NULL");
        }
    }

}
