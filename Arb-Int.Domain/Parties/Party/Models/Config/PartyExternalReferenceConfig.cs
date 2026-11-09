using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Parties.Configurations
{
    public class PartyExternalReferenceConfig :
        IEntityTypeConfiguration<PartyExternalReference>
    {
        public void Configure(
            EntityTypeBuilder<PartyExternalReference> builder)
        {
            builder.ToTable("PartyExternalReferences", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "CK_PartyExternalReferences_SeenDates",
                    "\"LastSeenUtc\" >= \"FirstSeenUtc\"");
            });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.SourceSystem)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.SourceRecordKey)
                .IsRequired()
                .HasMaxLength(512);

            builder.Property(x => x.SourceUrl)
                .HasMaxLength(2048);

            builder.Property(x => x.ContentHash)
                .HasMaxLength(128);

            builder.HasIndex(x => new
            {
                x.SourceSystem,
                x.SourceRecordKey
            })
            .IsUnique();

            builder.HasIndex(x => x.PartyId);

            builder.HasOne(x => x.Party)
                .WithMany(x => x.ExternalReferences)
                .HasForeignKey(x => x.PartyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}