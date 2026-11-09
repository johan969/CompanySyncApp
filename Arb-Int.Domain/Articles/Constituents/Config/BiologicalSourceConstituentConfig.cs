using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Articles.Configurations
{
    public class BiologicalSourceConstituentConfig :
        IEntityTypeConfiguration<BiologicalSourceConstituent>
    {
        public void Configure(
            EntityTypeBuilder<BiologicalSourceConstituent> builder)
        {
            builder.ToTable("BiologicalSourceConstituents");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.EvidenceLevel)
                .HasConversion<string>()
                .HasMaxLength(16)
                .HasDefaultValue(ContentEvidenceLevel.None);

            builder.Property(x => x.Notes)
                .HasMaxLength(4096);

            builder.HasIndex(x => new
            {
                x.BiologicalSourceId,
                x.ConstituentId
            })
            .IsUnique();

            builder.HasIndex(x => x.ConstituentId);

            builder.HasOne(x => x.BiologicalSource)
                .WithMany(x => x.ConstituentLinks)
                .HasForeignKey(x => x.BiologicalSourceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Constituent)
                .WithMany(x => x.BiologicalSourceLinks)
                .HasForeignKey(x => x.ConstituentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}