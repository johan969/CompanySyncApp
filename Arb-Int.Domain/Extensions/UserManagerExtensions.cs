using Arb_Int.Domain.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.Extensions
{
    public static class UserManagerExtensions
    {
        public static async Task<ApplicationUser?> FindByName(this UserManager<ApplicationUser> userManager, string name)
        {
            return await userManager.Users.FirstOrDefaultAsync(um => um.UserName == name);
        }

        public static async Task<ApplicationUser?> FindByEmail(this UserManager<ApplicationUser> userManager, string email)
        {
            return await userManager.Users.FirstOrDefaultAsync(um => um.Email == email);
        }
        public static async Task<ApplicationUser?> FindByName(this UserManager<ApplicationUser> userManager, string firstname, string lastname)
        {
            return await userManager.Users.FirstOrDefaultAsync(um => um.FirstName == firstname && um.LastName == lastname);
        }
    }
}