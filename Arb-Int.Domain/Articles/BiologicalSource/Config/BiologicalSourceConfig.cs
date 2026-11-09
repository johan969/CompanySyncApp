using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Articles.Configurations
{
    public class BiologicalSourceConfig :
        IEntityTypeConfiguration<BiologicalSource>
    {
        public void Configure(EntityTypeBuilder<BiologicalSource> builder)
        {
            builder.ToTable("BiologicalSources", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "CK_BiologicalSources_Parent_NotSelf",
                    "\"ParentBiologicalSourceId\" IS NULL " +
                    "OR \"ParentBiologicalSourceId\" <> \"Id\"");
            });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.NormalizedName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.Slug)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.Notes)
                .HasMaxLength(4096);

            builder.Property(x => x.RegulatoryRiskReason)
                .HasMaxLength(4096);

            builder.Property(x => x.TypicalHabitat)
                .HasMaxLength(4096);

            builder.Property(x => x.DistributionSummary)
                .HasMaxLength(4096);

            builder.Property(x => x.RecordKind)
                .HasConversion<string>()
                .HasMaxLength(48)
                .HasDefaultValue(BiologicalSourceRecordKind.Unknown);

            builder.Property(x => x.DeclaredRank)
                .HasConversion<string>()
                .HasMaxLength(32);

            builder.Property(x => x.RegulatoryRisk)
                .HasConversion<string>()
                .HasMaxLength(32);

            builder.HasIndex(x => x.Slug)
                .IsUnique();

            builder.HasIndex(x => x.TaxonId);

            builder.HasIndex(x => x.ParentBiologicalSourceId);

            builder.HasIndex(x => new
            {
                x.ParentBiologicalSourceId,
                x.DeclaredRank,
                x.NormalizedName
            });

            builder.HasOne(x => x.Taxon)
                .WithMany(x => x.BiologicalSources)
                .HasForeignKey(x => x.TaxonId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ParentBiologicalSource)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentBiologicalSourceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}