using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Crm.Configurations
{
    public class BusinessRelationshipConfig : IEntityTypeConfiguration<BusinessRelationship>
    {
        public void Configure(EntityTypeBuilder<BusinessRelationship> builder)
        {
            builder.ToTable("BusinessRelationships", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "CK_BusinessRelationships_Score",
                    "\"Score\" IS NULL " +
                    "OR (\"Score\" >= 0 AND \"Score\" <= 100)");

                tableBuilder.HasCheckConstraint(
                    "CK_BusinessRelationships_ContactDates",
                    """
                "LastContactedUtc" IS NULL
                OR "FirstContactedUtc" IS NULL
                OR "LastContactedUtc" >= "FirstContactedUtc"
                """);

                tableBuilder.HasCheckConstraint(
                    "CK_BusinessRelationships_ClosedDate",
                    "\"ClosedUtc\" IS NULL " +
                    "OR \"ClosedUtc\" >= \"FirstObservedUtc\"");
            });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Priority)
                .HasConversion<string>()
                .HasMaxLength(16)
                .HasDefaultValue(RelationshipPriority.Normal);

            builder.Property(x => x.LeadSource)
                .HasMaxLength(255);

            builder.Property(x => x.Notes)
                .HasMaxLength(4096);

            /*
             * En motpart får en aktuell relation per relationstyp och
             * intern organisation.
             *
             * Samma motpart kan alltså ha både Supplier och Buyer.
             */
            builder.HasIndex(x => new
            {
                x.OwningOrganizationId,
                x.CounterpartyId,
                x.BusinessRelationshipTypeId
            })
            .IsUnique();

            builder.HasIndex(x => x.CounterpartyId);

            builder.HasIndex(x => x.OwnerUserId);

            builder.HasIndex(x => new
            {
                x.BusinessRelationshipTypeId,
                x.BusinessRelationshipStageId
            });

            builder.HasIndex(x => x.NextActionUtc);

            builder.HasOne(x => x.OwningOrganization)
                .WithMany()
                .HasForeignKey(x => x.OwningOrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Counterparty)
                .WithMany()
                .HasForeignKey(x => x.CounterpartyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.BusinessRelationshipType)
                .WithMany(x => x.Relationships)
                .HasForeignKey(x => x.BusinessRelationshipTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            /*
             * Sammansatt FK säkerställer:
             *
             * BusinessRelationshipStage.BusinessRelationshipTypeId
             *     == BusinessRelationship.BusinessRelationshipTypeId
             */
            builder.HasOne(x => x.BusinessRelationshipStage)
                .WithMany(x => x.Relationships)
                .HasForeignKey(x => new
                {
                    x.BusinessRelationshipTypeId,
                    x.BusinessRelationshipStageId
                })
                .HasPrincipalKey(x => new
                {
                    x.BusinessRelationshipTypeId,
                    x.Id
                })
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}