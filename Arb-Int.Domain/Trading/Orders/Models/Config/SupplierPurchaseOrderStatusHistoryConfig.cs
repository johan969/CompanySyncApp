using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class SupplierPurchaseOrderStatusHistoryConfig : IEntityTypeConfiguration<SupplierPurchaseOrderStatusHistory>
    {
        public void Configure(EntityTypeBuilder<SupplierPurchaseOrderStatusHistory> builder)
        {
            builder.ToTable("SupplierPurchaseOrderStatusHistory");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.FromStatus)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.ToStatus)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.Reason)
                .HasMaxLength(4096)
                .IsRequired();

            builder.Property(x => x.Notes)
                .HasMaxLength(4096);

            builder.HasIndex(x => new
            {
                x.SupplierPurchaseOrderId,
                x.ChangedAtUtc
            });
        }
    }
}
