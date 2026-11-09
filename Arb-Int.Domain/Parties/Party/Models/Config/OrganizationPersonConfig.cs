using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Parties.Configurations
{
    public class OrganizationPersonConfig :
        IEntityTypeConfiguration<OrganizationPerson>
    {
        public void Configure(EntityTypeBuilder<OrganizationPerson> builder)
        {
            builder.ToTable("OrganizationPeople", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "CK_OrganizationPeople_ValidDates",
                    "\"ValidToUtc\" IS NULL " +
                    "OR \"ValidFromUtc\" IS NULL " +
                    "OR \"ValidToUtc\" >= \"ValidFromUtc\"");
            });

            builder.HasKey(x => x.Id);

            /*
             * Används av PartyContactPoint för att säkerställa att en
             * kontextuell kontaktuppgift tillhör rätt person.
             */
            builder.HasAlternateKey(x => new
            {
                x.Id,
                x.PersonId
            });

            builder.Property(x => x.RelationshipType)
                .HasConversion<string>()
                .HasMaxLength(32)
                .HasDefaultValue(
                    OrganizationPersonRelationshipType.Unknown);

            builder.Property(x => x.JobTitle)
                .HasMaxLength(255);

            builder.Property(x => x.Department)
                .HasMaxLength(255);

            builder.Property(x => x.Notes)
                .HasMaxLength(4096);

            builder.HasIndex(x => new
            {
                x.OrganizationId,
                x.PersonId
            })
            .IsUnique()
            .HasFilter("\"ValidToUtc\" IS NULL");

            builder.HasIndex(x => x.PersonId);

            builder.HasIndex(x => x.OrganizationId)
                .IsUnique()
                .HasFilter(
                    "\"IsPrimaryContact\" = TRUE " +
                    "AND \"ValidToUtc\" IS NULL");

            builder.HasOne(x => x.Organization)
                .WithMany(x => x.PersonLinks)
                .HasForeignKey(x => x.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Person)
                .WithMany(x => x.OrganizationLinks)
                .HasForeignKey(x => x.PersonId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}