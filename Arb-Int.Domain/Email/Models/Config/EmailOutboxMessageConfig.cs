using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Email.Configurations
{
    public class EmailOutboxMessageConfig : IEntityTypeConfiguration<EmailOutboxMessage>
    {
        public void Configure(EntityTypeBuilder<EmailOutboxMessage> b)
        {
            b.ToTable("EmailOutboxMessages");

            b.HasKey(x => x.Id);

            b.Property(x => x.Subject)
                .HasMaxLength(512)
                .IsRequired();

            b.Property(x => x.BodyHtml)
                .IsRequired();

            b.Property(x => x.FromEmailSnapshot)
                .HasMaxLength(320)
                .IsRequired();

            b.Property(x => x.FromNameSnapshot)
                .HasMaxLength(256);

            b.Property(x => x.ReplyToEmailSnapshot)
                .HasMaxLength(320);

            b.Property(x => x.ReplyToNameSnapshot)
                .HasMaxLength(256);

            b.Property(x => x.ProviderTemplateId)
                .HasMaxLength(256);

            b.Property(x => x.Provider)
                .HasConversion<string>()
                .HasMaxLength(32);

            b.Property(x => x.ContentMode)
                .HasConversion<string>()
                .HasMaxLength(32);

            b.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(32);

            b.Property(x => x.VariablesJson)
                .HasColumnType("text");

            b.Property(x => x.PersonalizationsJson)
                .HasColumnType("text");

            // Recipients
            b.HasMany(x => x.Recipients)
             .WithOne(r => r.OutboxMessage)
             .HasForeignKey(r => r.OutboxMessageId)
             .OnDelete(DeleteBehavior.Cascade);

            // 1–1 mot ScheduledTask om du kör det
            b.HasOne(x => x.ScheduledTask)
             .WithOne()
             .HasForeignKey<EmailOutboxMessage>(x => x.ScheduledTaskId)
             .OnDelete(DeleteBehavior.SetNull);

            b.HasIndex(x => new { x.PartyId, x.Status });
            b.HasIndex(x => x.FromUserId);
            b.HasIndex(x => x.ScheduledTaskId)
             .IsUnique()
             .HasFilter("\"ScheduledTaskId\" IS NOT NULL");
        }
    }
}