using Arb_Int.Domain;
using Arb_Int.Domain.Errors;
using Arb_Int.Domain.User;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public interface IUserSettingsService
{
    Task<Dictionary<Guid, UserInfoResponse>> GetByUserIdsAsync(IEnumerable<Guid> userIds, CancellationToken cancellationToken = default);
    Task<Result<List<string>>> GetAvailableRolesAsync(CancellationToken cancellationToken = default);
}
public class UserSettingsService : IUserSettingsService
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly RoleManager<ApplicationRole> roleManager;
    private readonly Arb_IntDbContext dbContext;

    public UserSettingsService(
        Arb_IntDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        this.dbContext = dbContext;
        this.userManager = userManager;
        this.roleManager = roleManager;
    }

    public async Task<Result<List<string>>> GetAvailableRolesAsync(CancellationToken cancellationToken = default)
    {
        var roles = await roleManager.Roles.Where(r => !string.IsNullOrEmpty(r.Name)).Select(r => r.Name!).ToListAsync(cancellationToken);
        if (roles != null)
            return Result.Ok(roles!);
        return Result.Fail(new NotFoundError(nameof(ApplicationRole), 0, NotFoundType.Collection));
    }

    public async Task<Dictionary<Guid, UserInfoResponse>> GetByUserIdsAsync(IEnumerable<Guid> userIds, CancellationToken cancellationToken = default)
    {
        var ids = userIds.Distinct().Where(x => x != Guid.Empty).ToList();
        if (ids.Count == 0) return [];

        var list = await dbContext.Set<ApplicationUser>()
            .Where(x => ids.Contains(x.Id))
            .Select(x => new UserInfoResponse
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                UserName = x.UserName ?? "N/A",
                Email = x.Email ?? "N/A",
            })
            .ToListAsync(cancellationToken);
        return list.ToDictionary(x => x.Id);
    }
}