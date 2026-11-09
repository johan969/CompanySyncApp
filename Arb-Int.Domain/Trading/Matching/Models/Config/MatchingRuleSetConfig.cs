using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class MatchingRuleSetConfig : IEntityTypeConfiguration<MatchingRuleSet>
    {
        public void Configure(EntityTypeBuilder<MatchingRuleSet> builder)
        {
            builder.ToTable("MatchingRuleSets");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.Code)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.HasIndex(x => x.Code)
                .IsUnique();

            builder.HasIndex(x => x.Status);

            builder.HasMany(x => x.Versions)
                .WithOne(x => x.MatchingRuleSet)
                .HasForeignKey(x => x.MatchingRuleSetId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
