
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
namespace Arb_Int.Domain.User
{
    public interface ICurrentIdentityService
    {
        ClaimsPrincipal? ClaimsPrincipal { get; }
        Guid? UserId { get; }
        bool IsAuthenticated { get; }
    }
    public class CurrentIdentityService : ICurrentIdentityService
    {
        private readonly IHttpContextAccessor? _httpContextAccessor;
        private Guid? OverriddenUserId { get; set; }
        private Guid? OverriddenOnBehalfOfUserId { get; set; }
        public CurrentIdentityService(IHttpContextAccessor? httpContextAccessor = null)
        {
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

        public bool IsAuthenticated => _httpContextAccessor?.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
        public ClaimsPrincipal? ClaimsPrincipal
        {
            get
            {
                return _httpContextAccessor?.HttpContext?.User;
            }
        }
    }
}