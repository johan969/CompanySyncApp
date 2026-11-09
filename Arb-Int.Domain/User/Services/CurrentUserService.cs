using System.Collections.Immutable;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Arb_Int.Domain.User
{
    public interface ICurrentUserService
    {
        ClaimsPrincipal? ClaimsPrincipal { get; }
        Guid? UserId { get; }
        bool IsAuthenticated { get; }
        IReadOnlyCollection<string> Roles { get; }
        IReadOnlyDictionary<string, object?> Variables { get; }

    }
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor? _httpContextAccessor;
        private readonly IUserSettingsService _userSettingsService;

        public CurrentUserService(IUserSettingsService userSettingsService, IHttpContextAccessor? httpContextAccessor = null)
        {
            _userSettingsService = userSettingsService;
            _httpContextAccessor = httpContextAccessor;
        }
        public Guid? UserId
        {
            get
            {
                var user = _httpContextAccessor?.HttpContext?.User;
                if (user == null || (!user.Identity?.IsAuthenticated ?? false))
                    return null;
                var idClaim = user?.FindFirst(ClaimTypes.NameIdentifier); // or "sub" for JWT
                if (idClaim == null || !Guid.TryParse(idClaim.Value, out var userId))
                    return null;
                return userId;
            }
        }
        public ClaimsPrincipal? ClaimsPrincipal
        {
            get
            {
                return _httpContextAccessor?.HttpContext?.User;
            }
        }
        public bool IsAuthenticated => _httpContextAccessor?.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
        public IReadOnlyCollection<string> Roles => ClaimsPrincipal?.Claims?.Where(c => c.Type == "role").Select(r => r.Value).ToImmutableList() ?? [];
        public IReadOnlyDictionary<string, object?> Variables
        {
            get
            {
                var dict = new Dictionary<string, object?>();
                if (ClaimsPrincipal != null)
                {
                    dict.Add("UserId", UserId);
                    dict.Add("Roles", Roles);
                }
                return dict;
            }
        }


    }
}