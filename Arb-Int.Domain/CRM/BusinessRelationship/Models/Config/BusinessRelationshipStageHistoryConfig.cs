using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Crm.Configurations
{
    public class BusinessRelationshipStageHistoryConfig : IEntityTypeConfiguration<BusinessRelationshipStageHistory>
    {
        public void Configure(EntityTypeBuilder<BusinessRelationshipStageHistory> builder)
        {
            builder.ToTable("BusinessRelationshipStageHistory");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Notes)
                .HasMaxLength(4096);

            builder.HasIndex(x => new
            {
                x.BusinessRelationshipId,
                x.ChangedUtc
            });

            builder.HasOne(x => x.BusinessRelationship)
                .WithMany(x => x.StageHistory)
                .HasForeignKey(x => x.BusinessRelationshipId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.FromStage)
                .WithMany()
                .HasForeignKey(x => x.FromStageId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ToStage)
                .WithMany()
                .HasForeignKey(x => x.ToStageId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}