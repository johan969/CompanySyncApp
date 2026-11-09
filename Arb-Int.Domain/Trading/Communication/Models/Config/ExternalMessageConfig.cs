using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class ExternalMessageConfig : IEntityTypeConfiguration<ExternalMessage>
    {
        public void Configure(EntityTypeBuilder<ExternalMessage> builder)
        {
            builder.ToTable("ExternalMessages");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.Direction)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.Channel)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.ExternalMessageId)
                .HasMaxLength(512);

            builder.Property(x => x.Subject)
                .HasMaxLength(500);

            builder.Property(x => x.Body)
                .HasMaxLength(32000)
                .IsRequired();

            builder.Property(x => x.ContentHash)
                .HasMaxLength(128);

            builder.HasIndex(x => new
            {
                x.ExternalConversationId,
                x.OccurredAtUtc
            });

            builder.HasIndex(x => x.ExternalMessageId);

            builder.HasOne(x => x.InReplyToMessage)
                .WithMany(x => x.Replies)
                .HasForeignKey(x => x.InReplyToMessageId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.SenderPerson)
                .WithMany()
                .HasForeignKey(x => x.SenderPersonId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.RecipientPerson)
                .WithMany()
                .HasForeignKey(x => x.RecipientPersonId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.DisclosurePackageRevision)
                .WithMany()
                .HasForeignKey(x => x.DisclosurePackageRevisionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Attachments)
                .WithOne(x => x.ExternalMessage)
                .HasForeignKey(x => x.ExternalMessageId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
