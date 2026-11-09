using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Crm.Configurations
{
    public class CrmActivityConfig : IEntityTypeConfiguration<CrmActivity>
    {
        public void Configure(EntityTypeBuilder<CrmActivity> builder)
        {
            builder.ToTable("CrmActivities", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "CK_CrmActivities_Completed",
                    """
                "Status" <> 'Completed'
                OR "CompletedUtc" IS NOT NULL
                """);
            });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ActivityType)
                .HasConversion<string>()
                .HasMaxLength(32)
                .HasDefaultValue(CrmActivityType.Unknown);

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(32)
                .HasDefaultValue(CrmActivityStatus.Planned);

            builder.Property(x => x.Direction)
                .HasConversion<string>()
                .HasMaxLength(16)
                .HasDefaultValue(CrmActivityDirection.None);

            builder.Property(x => x.Subject)
                .IsRequired()
                .HasMaxLength(512);

            builder.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(8192);

            builder.Property(x => x.ExternalSystem)
                .HasMaxLength(100);

            builder.Property(x => x.ExternalRecordId)
                .HasMaxLength(512);

            builder.HasIndex(x => new
            {
                x.ExternalSystem,
                x.ExternalRecordId
            })
            .IsUnique()
            .HasFilter(
                "\"ExternalSystem\" IS NOT NULL " +
                "AND \"ExternalRecordId\" IS NOT NULL");

            builder.HasIndex(x => new
            {
                x.RegardingPartyId,
                x.OccurredUtc
            });

            builder.HasIndex(x => new
            {
                x.OwnerUserId,
                x.Status,
                x.DueUtc
            });

            builder.HasIndex(x => x.BusinessRelationshipId);

            builder.HasOne(x => x.RegardingParty)
                .WithMany()
                .HasForeignKey(x => x.RegardingPartyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.BusinessRelationship)
                .WithMany(x => x.Activities)
                .HasForeignKey(x => x.BusinessRelationshipId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}