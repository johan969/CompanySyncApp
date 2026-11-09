using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class IncotermRuleConfig : IEntityTypeConfiguration<IncotermRule>
    {
        public void Configure(EntityTypeBuilder<IncotermRule> builder)
        {
            builder.ToTable("IncotermRules");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.Code)
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(x => x.Version)
                .HasMaxLength(20)
                .IsRequired();

            builder.HasIndex(x => new
            {
                x.Code,
                x.Version
            })
                .IsUnique();

            builder.HasIndex(x => x.IsActive);
        }
    }
}
