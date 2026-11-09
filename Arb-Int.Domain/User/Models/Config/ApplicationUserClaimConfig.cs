using Arb_Int.Domain.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
public sealed class ApplicationUserClaimConfiguration : IEntityTypeConfiguration<ApplicationUserClaim>
{
    public void Configure(EntityTypeBuilder<ApplicationUserClaim> builder)
    {
        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("IX_AspNetUserClaims_UserId");

        builder.HasIndex(x => new { x.UserId, x.ClaimType, x.ClaimValue })
            .HasDatabaseName("IX_AspNetUserClaims_UserId_ClaimType_ClaimValue");

        builder.ToTable("UserClaim");
    }
}