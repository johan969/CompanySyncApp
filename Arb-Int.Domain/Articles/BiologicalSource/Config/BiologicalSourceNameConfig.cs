using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Articles.Configurations
{
    public class BiologicalSourceNameConfig :
        IEntityTypeConfiguration<BiologicalSourceName>
    {
        public void Configure(EntityTypeBuilder<BiologicalSourceName> builder)
        {
            builder.ToTable("BiologicalSourceNames");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.NameType)
                .HasConversion<string>()
                .HasMaxLength(32)
                .HasDefaultValue(BiologicalSourceNameType.Unknown);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.NormalizedName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.LanguageCode)
                .IsRequired()
                .HasMaxLength(16)
                .HasDefaultValue("und");

            builder.HasIndex(x => new
            {
                x.BiologicalSourceId,
                x.NameType,
                x.LanguageCode,
                x.NormalizedName
            })
            .IsUnique();

            builder.HasOne(x => x.BiologicalSource)
                .WithMany(x => x.AlternativeNames)
                .HasForeignKey(x => x.BiologicalSourceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}