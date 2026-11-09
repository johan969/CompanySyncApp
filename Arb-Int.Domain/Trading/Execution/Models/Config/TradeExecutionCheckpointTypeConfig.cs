using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class TradeExecutionCheckpointTypeConfig : IEntityTypeConfiguration<TradeExecutionCheckpointType>
    {
        public void Configure(EntityTypeBuilder<TradeExecutionCheckpointType> builder)
        {
            builder.ToTable("TradeExecutionCheckpointTypes");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.Code)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.DefaultCompletionMode)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.DefaultAutomationHandlerKey)
                .HasMaxLength(255);

            builder.HasIndex(x => x.Code)
                .IsUnique();

            builder.HasIndex(x => new
            {
                x.IsActive,
                x.DefaultSortOrder
            });

            builder.HasMany(x => x.Checkpoints)
                .WithOne(x => x.TradeExecutionCheckpointType)
                .HasForeignKey(x => x.TradeExecutionCheckpointTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
