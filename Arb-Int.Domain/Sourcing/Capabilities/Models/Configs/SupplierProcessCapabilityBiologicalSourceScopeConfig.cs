using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb_Int.Domain.Sourcing.Configurations
{
    public class
        SupplierProcessCapabilityBiologicalSourceScopeConfig : IEntityTypeConfiguration<SupplierProcessCapabilityBiologicalSourceScope>
    {
        public void Configure(EntityTypeBuilder<SupplierProcessCapabilityBiologicalSourceScope> builder)
        {
            builder.ToTable(
                "SupplierProcessCapabilityBiologicalSourceScopes");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Notes)
                .HasMaxLength(4096);

            builder.HasIndex(x => new
            {
                x.SupplierProcessCapabilityId,
                x.BusinessRelationshipBiologicalSourceClaimId
            })
            .IsUnique();

            builder.HasIndex(
                x => x.BusinessRelationshipBiologicalSourceClaimId);

            /*
             * Processkapaciteten måste tillhöra samma relation.
             */
            builder.HasOne(x => x.SupplierProcessCapability)
                .WithMany(x => x.BiologicalSourceScopes)
                .HasForeignKey(x => new
                {
                    x.SupplierProcessCapabilityId,
                    x.BusinessRelationshipId
                })
                .HasPrincipalKey(x => new
                {
                    x.Id,
                    x.BusinessRelationshipId
                })
                .OnDelete(DeleteBehavior.Cascade);

            /*
             * Även den biologiska claimen måste tillhöra samma relation.
             */
            builder.HasOne(x => x.BiologicalSourceClaim)
                .WithMany()
                .HasForeignKey(x => new
                {
                    x.BusinessRelationshipBiologicalSourceClaimId,
                    x.BusinessRelationshipId
                })
                .HasPrincipalKey(x => new
                {
                    x.Id,
                    x.BusinessRelationshipId
                })
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}