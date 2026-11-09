
using Arb_Int.Domain.Base;
using Arb_Int.Domain.Parties;
using Arb_Int.Domain.Worker;
using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace Arb_Int.Domain.User
{
    public class ApplicationUser : IdentityUser<Guid>, ICreateTracker, IActionTracker
    {

        [PersonalData]
        public string? FirstName { get; set; }
        [PersonalData]
        public string? LastName { get; set; }

        [JsonIgnore]
        public List<RefreshToken> RefreshTokens { get; set; } = [];

        public bool IsPasswordChangeRequired { get; set; }
        public bool IsInactivated { get; set; }
        public DateTimeOffset? InactivatedTime { get; set; }
        public Guid? InactivatedByUserId { get; set; }
        public Guid? InactivatedByOnBehalfOfUserId { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }


        public ICollection<ApplicationUserClaim> Claims { get; set; } = [];
        public ICollection<ApplicationUserLogin> Logins { get; set; } = [];
        public ICollection<ApplicationUserToken> Tokens { get; set; } = [];
        public ICollection<ApplicationUserRole> UserRoles { get; set; } = [];
        public ICollection<ScheduledTask> ScheduledTasks { get; set; } = [];
        public PersonUserAccount? PersonAccount { get; set; }
    }

}
