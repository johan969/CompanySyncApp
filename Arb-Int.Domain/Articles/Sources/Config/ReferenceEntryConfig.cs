using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Articles.Configurations
{
    public class ReferenceEntryConfig :
        IEntityTypeConfiguration<ReferenceEntry>
    {
        public void Configure(EntityTypeBuilder<ReferenceEntry> builder)
        {
            builder.ToTable("ReferenceEntries");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Type)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(1024);

            builder.Property(x => x.Url)
                .HasMaxLength(2048);

            builder.Property(x => x.Notes)
                .HasMaxLength(4096);

            builder.Property(x => x.ReferenceKey)
                .IsRequired()
                .HasMaxLength(128);

            builder.HasIndex(x => x.ReferenceKey)
                .IsUnique();
        }
    }
}