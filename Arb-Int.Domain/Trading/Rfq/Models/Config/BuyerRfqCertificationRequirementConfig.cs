using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading.Configurations
{
    public class BuyerRfqCertificationRequirementConfig : IEntityTypeConfiguration<BuyerRfqCertificationRequirement>
    {
        public void Configure(EntityTypeBuilder<BuyerRfqCertificationRequirement> builder)
        {
            builder.Property(x => x.Operator)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.RequiredScope)
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.HasOne(x => x.CertificationScheme)
                .WithMany()
                .HasForeignKey(x => x.CertificationSchemeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.CertificationSchemeJurisdiction)
                .WithMany()
                .HasForeignKey(x => new
                {
                    x.CertificationSchemeJurisdictionId,
                    x.CertificationSchemeId
                })
                .HasPrincipalKey(x => new
                {
                    x.Id,
                    x.CertificationSchemeId
                })
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
