using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Parties.Configurations
{
    public class PartyContactPointConfig :
        IEntityTypeConfiguration<PartyContactPoint>
    {
        public void Configure(EntityTypeBuilder<PartyContactPoint> builder)
        {
            builder.ToTable("PartyContactPoints", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "CK_PartyContactPoints_ValidDates",
                    "\"ValidToUtc\" IS NULL " +
                    "OR \"ValidToUtc\" >= \"ValidFromUtc\"");
            });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Kind)
                .HasConversion<string>()
                .HasMaxLength(32)
                .HasDefaultValue(ContactPointKind.Unknown);

            builder.Property(x => x.Label)
                .HasMaxLength(255);

            builder.Property(x => x.Value)
                .IsRequired()
                .HasMaxLength(2048);

            builder.Property(x => x.NormalizedValue)
                .IsRequired()
                .HasMaxLength(2048);

            builder.Property(x => x.Extension)
                .HasMaxLength(50);

            builder.Property(x => x.PermissionStatus)
                .HasConversion<string>()
                .HasMaxLength(32)
                .HasDefaultValue(ContactPermissionStatus.Unknown);

            builder.Property(x => x.Notes)
                .HasMaxLength(4096);

            /*
             * Unika aktiva, okontextualiserade kontaktuppgifter.
             *
             * Exempel:
             * Företagets växel eller personens privata e-post.
             */
            builder.HasIndex(x => new
            {
                x.PartyId,
                x.Kind,
                x.NormalizedValue
            })
            .IsUnique()
            .HasFilter(
                "\"OrganizationPersonId\" IS NULL " +
                "AND \"ValidToUtc\" IS NULL");

            /*
             * Unika aktiva arbetskontaktuppgifter inom en viss
             * OrganizationPerson-relation.
             */
            builder.HasIndex(x => new
            {
                x.OrganizationPersonId,
                x.Kind,
                x.NormalizedValue
            })
            .IsUnique()
            .HasFilter(
                "\"OrganizationPersonId\" IS NOT NULL " +
                "AND \"ValidToUtc\" IS NULL");

            builder.HasIndex(x => new
            {
                x.PartyId,
                x.Kind
            })
            .IsUnique()
            .HasFilter(
                "\"IsPrimary\" = TRUE " +
                "AND \"OrganizationPersonId\" IS NULL " +
                "AND \"ValidToUtc\" IS NULL");

            builder.HasIndex(x => new
            {
                x.OrganizationPersonId,
                x.Kind
            })
            .IsUnique()
            .HasFilter(
                "\"IsPrimary\" = TRUE " +
                "AND \"OrganizationPersonId\" IS NOT NULL " +
                "AND \"ValidToUtc\" IS NULL");

            builder.HasOne(x => x.Party)
                .WithMany(x => x.ContactPoints)
                .HasForeignKey(x => x.PartyId)
                .OnDelete(DeleteBehavior.Cascade);

            /*
             * FK:
             *
             * PartyContactPoint.OrganizationPersonId
             *     -> OrganizationPerson.Id
             *
             * PartyContactPoint.PartyId
             *     -> OrganizationPerson.PersonId
             *
             * Därmed kan en organisationskontext inte av misstag kopplas
             * till en annan person.
             */
            builder.HasOne(x => x.OrganizationPerson)
                .WithMany(x => x.ContextualContactPoints)
                .HasForeignKey(x => new
                {
                    x.OrganizationPersonId,
                    x.PartyId
                })
                .HasPrincipalKey(x => new
                {
                    x.Id,
                    x.PersonId
                })
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}