using Arb_Int.Domain.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class ApplicationUserRoleConfiguration : IEntityTypeConfiguration<ApplicationUserRole>
{
    public void Configure(EntityTypeBuilder<ApplicationUserRole> builder)
    {
        builder.HasKey(x => new { x.UserId, x.RoleId });

        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("IX_AspNetUserRoles_UserId");

        builder.HasIndex(x => x.RoleId)
            .HasDatabaseName("IX_AspNetUserRoles_RoleId");

        builder.HasOne(x => x.User)
            .WithMany(x => x.UserRoles)
            .HasForeignKey(x => x.UserId)
            .IsRequired();

        builder.HasOne(x => x.Role)
            .WithMany(x => x.UserRoles)
            .HasForeignKey(x => x.RoleId)
            .IsRequired();

        builder.ToTable("UserRole");
    }
}