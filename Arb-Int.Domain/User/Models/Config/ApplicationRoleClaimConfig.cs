using Arb_Int.Domain.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class ApplicationRoleClaimConfiguration : IEntityTypeConfiguration<ApplicationRoleClaim>
{
    public void Configure(EntityTypeBuilder<ApplicationRoleClaim> builder)
    {
        builder.HasIndex(x => x.RoleId)
            .HasDatabaseName("IX_AspNetRoleClaims_RoleId");

        // Bra om du ibland söker/filterar på specifika rollclaims
        builder.HasIndex(x => new { x.RoleId, x.ClaimType, x.ClaimValue })
            .HasDatabaseName("IX_AspNetRoleClaims_RoleId_ClaimType_ClaimValue");

        builder.HasOne(x => x.Role)
            .WithMany(x => x.RoleClaims)
            .HasForeignKey(x => x.RoleId)
            .IsRequired();
        builder.ToTable("RoleClaims");
    }
}