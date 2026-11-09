using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class ShipmentMilestoneTypeConfig : IEntityTypeConfiguration<ShipmentMilestoneType>
    {
        public void Configure(EntityTypeBuilder<ShipmentMilestoneType> builder)
        {
            builder.ToTable("ShipmentMilestoneTypes");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.Code)
                .HasMaxLength(100)
                .IsRequired();

            builder.HasIndex(x => x.Code)
                .IsUnique();

            builder.HasIndex(x => new
            {
                x.IsActive,
                x.SortOrder
            });
        }
    }
}
