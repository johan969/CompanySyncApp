using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class ExternalSystemConfig : IEntityTypeConfiguration<ExternalSystem>
    {
        public void Configure(EntityTypeBuilder<ExternalSystem> builder)
        {
            builder.ToTable("ExternalSystems");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.Code)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Kind)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.BaseUrl)
                .HasMaxLength(2048);

            builder.HasIndex(x => x.Code)
                .IsUnique();

            builder.HasIndex(x => new
            {
                x.Kind,
                x.IsActive
            });
        }
    }
}
