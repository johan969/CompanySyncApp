using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Crm.Configurations
{
    public class
        BusinessRelationshipBiologicalSourceClaimEvidenceConfig : IEntityTypeConfiguration<BusinessRelationshipBiologicalSourceClaimEvidence>
    {
        public void Configure(EntityTypeBuilder<BusinessRelationshipBiologicalSourceClaimEvidence> builder)
        {
            builder.ToTable(
                "BusinessRelationshipBiologicalSourceClaimEvidence");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.EvidenceType)
                .HasConversion<string>()
                .HasMaxLength(32)
                .HasDefaultValue(ClaimEvidenceType.Unknown);

            builder.Property(x => x.SourceSystem)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.SourceRecordKey)
                .IsRequired()
                .HasMaxLength(512);

            builder.Property(x => x.SourceUrl)
                .HasMaxLength(2048);

            builder.Property(x => x.Notes)
                .HasMaxLength(4096);

            builder.Property(x => x.ContentHash)
                .HasMaxLength(128);

            builder.HasIndex(x => new
            {
                x.BusinessRelationshipBiologicalSourceClaimId,
                x.SourceSystem,
                x.SourceRecordKey
            })
            .IsUnique();

            builder.HasOne(x => x.Claim)
                .WithMany(x => x.Evidence)
                .HasForeignKey(
                    x => x.BusinessRelationshipBiologicalSourceClaimId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}