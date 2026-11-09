using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arb_Int.Domain.User
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        [Column(Order = 10)]
        public string Description { get; set; } = string.Empty;
        public ICollection<ApplicationUserRole> UserRoles { get; set; } = [];
        public ICollection<ApplicationRoleClaim> RoleClaims { get; set; } = [];
        public int SortOrder { get; set; }

    }
    public class ApplicationUserRole : IdentityUserRole<Guid>
    {
        public ApplicationUser? User { get; set; }
        public ApplicationRole? Role { get; set; }
    }
}
