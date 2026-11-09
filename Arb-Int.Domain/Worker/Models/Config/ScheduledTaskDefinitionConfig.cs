
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Worker.Configurations
{
    public sealed class ScheduledTaskDefinitionConfig : IEntityTypeConfiguration<ScheduledTaskDefinition>
    {
        public void Configure(EntityTypeBuilder<ScheduledTaskDefinition> builder)
        {
            builder.ToTable("ScheduledTaskDefinitions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TaskType).HasConversion<string>().HasMaxLength(64);
            builder.Property(x => x.TriggerType).HasConversion<string>().HasMaxLength(64);
            builder.Property(x => x.Priority).HasConversion<string>().HasMaxLength(16);
            builder.Property(x => x.TimeSemantics).HasConversion<string>().HasMaxLength(32);
            builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(16);

            builder.Property(x => x.TimeZoneId).HasMaxLength(64);

            builder.Property(x => x.PayloadJson).HasColumnType("text");

            builder.Property(x => x.LockedBy).HasMaxLength(128);

            // Relations
            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.ClientCascade);

            builder.HasOne(x => x.Party)
                .WithMany()
                .HasForeignKey(x => x.PartyId)
                .OnDelete(DeleteBehavior.ClientCascade);

            builder.HasOne(x => x.ContactPoint)
                .WithMany()
                .HasForeignKey(x => x.ContactPointId)
                .OnDelete(DeleteBehavior.ClientCascade);

            builder.HasMany(x => x.Occurrences)
                .WithOne(o => o.Definition)
                .HasForeignKey(o => o.DefinitionId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.Property(x => x.IdempotencyKey).HasMaxLength(256);

            builder.Property(x => x.EndAtUtc);

            builder.HasIndex(x => new { x.PartyId, x.TaskType, x.IdempotencyKey })
                .IsUnique()
                .HasFilter("\"IdempotencyKey\" IS NOT NULL");
            // Indexes for claiming
            builder.HasIndex(x => new { x.Status, x.NextRunAtUtc, x.EndAtUtc, x.Id })
                .HasDatabaseName("IX_ScheduledTaskDefinitions_Claim");

            builder.HasIndex(x => x.LockedUntilUtc)
                .HasDatabaseName("IX_ScheduledTaskDefinitions_LockedUntilUtc");

            builder.HasIndex(x => new { x.PartyId, x.Status, x.NextRunAtUtc })
                .HasDatabaseName("IX_ScheduledTaskDefinitions_Organisation_Status_NextRunAtUtc");

            builder.HasIndex(x => new { x.ContactPointId, x.Status, x.NextRunAtUtc })
                .HasDatabaseName("IX_ScheduledTaskDefinitions_Contact_Status_NextRunAtUtc");
        }
    }
}
