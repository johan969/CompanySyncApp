using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Crm.Configurations
{
    public class CrmActivityParticipantConfig : IEntityTypeConfiguration<CrmActivityParticipant>
    {
        public void Configure(EntityTypeBuilder<CrmActivityParticipant> builder)
        {
            builder.ToTable("CrmActivityParticipants");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Role)
                .HasConversion<string>()
                .HasMaxLength(32)
                .HasDefaultValue(
                    CrmActivityParticipantRole.Unknown);

            builder.HasIndex(x => new
            {
                x.CrmActivityId,
                x.PartyId,
                x.Role
            })
            .IsUnique();

            builder.HasIndex(x => x.PartyId);

            builder.HasOne(x => x.CrmActivity)
                .WithMany(x => x.Participants)
                .HasForeignKey(x => x.CrmActivityId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Party)
                .WithMany()
                .HasForeignKey(x => x.PartyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}