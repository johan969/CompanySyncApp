using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Articles.Configurations
{
    public class MaterialConstituentConfig :
        IEntityTypeConfiguration<MaterialConstituent>
    {
        public void Configure(EntityTypeBuilder<MaterialConstituent> builder)
        {
            builder.ToTable("MaterialConstituents");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.EvidenceLevel)
                .HasConversion<string>()
                .HasMaxLength(16)
                .HasDefaultValue(ContentEvidenceLevel.None);

            builder.Property(x => x.Notes)
                .HasMaxLength(4096);

            builder.HasIndex(x => new
            {
                x.MaterialId,
                x.ConstituentId
            })
            .IsUnique();

            builder.HasIndex(x => x.ConstituentId);

            builder.HasOne(x => x.Material)
                .WithMany(x => x.ConstituentLinks)
                .HasForeignKey(x => x.MaterialId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Constituent)
                .WithMany(x => x.MaterialLinks)
                .HasForeignKey(x => x.ConstituentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}