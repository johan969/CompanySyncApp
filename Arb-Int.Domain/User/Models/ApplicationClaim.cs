using Microsoft.AspNetCore.Identity;

namespace Arb_Int.Domain.User
{
    public class ApplicationUserClaim : IdentityUserClaim<Guid>
    {
        public ApplicationUser? User { get; set; }
    }

    public class ApplicationRoleClaim : IdentityRoleClaim<Guid>
    {
        public ApplicationRole? Role { get; set; }
    }

    public class ApplicationUserToken : IdentityUserToken<Guid>
    {
        public ApplicationUser? User { get; set; }
    }
    public class ApplicationUserLogin : IdentityUserLogin<Guid>
    {
        public ApplicationUser? User { get; set; }
    }
}
