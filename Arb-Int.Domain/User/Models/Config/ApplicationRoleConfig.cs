
using Arb_Int.Domain.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.HasMany(e => e.UserRoles)
            .WithOne(e => e.Role)
            .HasForeignKey(ur => ur.RoleId)
            .IsRequired();

        // Each Role can have many associated RoleClaims
        builder.HasMany(e => e.RoleClaims)
            .WithOne(e => e.Role)
            .HasForeignKey(rc => rc.RoleId)
            .IsRequired();

        builder.HasIndex(x => x.NormalizedName)
            .HasDatabaseName("RoleNameIndex")
            .IsUnique()
            .HasFilter("\"NormalizedName\" IS NOT NULL");

        // Bra om du ofta sorterar/listar roller
        builder.HasIndex(x => x.SortOrder)
            .HasDatabaseName("IX_AspNetRoles_SortOrder");

        builder.ToTable("Role");
    }
}