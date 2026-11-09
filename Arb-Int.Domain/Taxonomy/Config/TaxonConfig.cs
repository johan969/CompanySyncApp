using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Taxonomy.Configurations
{
    public class TaxonConfig : IEntityTypeConfiguration<Taxon>
    {
        public void Configure(EntityTypeBuilder<Taxon> builder)
        {
            builder.ToTable("Taxon", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "CK_Taxon_ParentTaxonId_NotSelf",
                    "\"ParentTaxonId\" IS NULL OR \"ParentTaxonId\" <> \"Id\"");

                tableBuilder.HasCheckConstraint(
                    "CK_Taxon_AcceptedTaxonId_NotSelf",
                    "\"AcceptedTaxonId\" IS NULL OR \"AcceptedTaxonId\" <> \"Id\"");
            });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.NormalizedName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.Slug)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.PathKey)
                .IsRequired()
                .HasMaxLength(1024);

            builder.Property(x => x.Rank)
                .HasConversion<string>()
                .HasMaxLength(32)
                .HasDefaultValue(TaxonRank.Unknown);

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(32)
                .HasDefaultValue(TaxonomicStatus.Unknown);

            builder.HasIndex(x => x.PathKey)
                .IsUnique();

            builder.HasIndex(x => new
            {
                x.ParentTaxonId,
                x.Rank,
                x.NormalizedName
            })
                .IsUnique();

            builder.HasIndex(x => x.AcceptedTaxonId);

            builder.HasOne(x => x.ParentTaxon)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentTaxonId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.AcceptedTaxon)
                .WithMany(x => x.Synonyms)
                .HasForeignKey(x => x.AcceptedTaxonId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}