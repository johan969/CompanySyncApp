using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Email.Configurations
{
    public class EmailOutboxRecipientConfig : IEntityTypeConfiguration<EmailOutboxRecipient>
    {
        public void Configure(EntityTypeBuilder<EmailOutboxRecipient> b)
        {
            b.ToTable("EmailOutboxRecipients");

            b.HasKey(x => x.Id);

            b.Property(x => x.EmailSnapshot)
                .HasMaxLength(320)
                .IsRequired();

            b.Property(x => x.NameSnapshot)
                .HasMaxLength(256);

            b.Property(x => x.PersonalizationJson)
                .HasColumnType("text");

            b.Property(x => x.Type)
                .HasConversion<string>()
                .HasMaxLength(8);

            b.HasIndex(x => new { x.OutboxMessageId, x.Type });
            b.HasIndex(x => x.UserId);

            // Unik: samma user får inte ligga dubbelt i samma To/Cc/Bcc för samma outbox
            b.HasIndex(x => new { x.OutboxMessageId, x.Type, x.UserId })
             .IsUnique();
        }
    }
}