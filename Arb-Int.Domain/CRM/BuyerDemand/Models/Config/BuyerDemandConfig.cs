using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Crm.Configurations
{
    public class BuyerDemandConfig : IEntityTypeConfiguration<BuyerDemand>
    {
        public void Configure(EntityTypeBuilder<BuyerDemand> builder)
        {
            builder.ToTable(
                "BuyerDemands",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_BuyerDemands_Confidence",
                        """
                    "Confidence" IS NULL
                    OR ("Confidence" >= 0 AND "Confidence" <= 1)
                    """);

                    table.HasCheckConstraint(
                        "CK_BuyerDemands_Validity",
                        """
                    "ValidToUtc" IS NULL
                    OR "ValidFromUtc" IS NULL
                    OR "ValidToUtc" >= "ValidFromUtc"
                    """);
                });
            builder.HasKey(x => x.Id);

            builder.HasAlternateKey(x => new
            {
                x.Id,
                x.BusinessRelationshipId
            });
            builder.Property(x => x.ActionName)
                .HasMaxLength(255);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.KnowledgeLevel)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.Confidence)
                .HasPrecision(9, 6);

            builder.Property(x => x.InternalNotes)
                .HasMaxLength(4096);

            builder.HasIndex(x => new
            {
                x.BusinessRelationshipId,
                x.Status
            });

            builder.HasIndex(x => new
            {
                x.KnowledgeLevel,
                x.LastConfirmedAtUtc
            });

            builder.HasOne(x => x.BusinessRelationship)
                .WithMany(x => x.BuyerDemands)
                .HasForeignKey(x => x.BusinessRelationshipId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Scopes)
                .WithOne(x => x.BuyerDemand)
                .HasForeignKey(x => new
                {
                    x.BuyerDemandId,
                    x.BusinessRelationshipId
                })
                .HasPrincipalKey(x => new
                {
                    x.Id,
                    x.BusinessRelationshipId
                })
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Evidence)
                .WithOne(x => x.BuyerDemand)
                .HasForeignKey(x => x.BuyerDemandId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.StateHistory)
                .WithOne(x => x.BuyerDemand)
                .HasForeignKey(x => x.BuyerDemandId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
