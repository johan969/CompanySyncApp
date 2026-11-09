using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Arb_Int.Domain.User
{
    public class RefreshToken
    {
        [Key]
        [JsonIgnore]
        public int Id { get; set; }


        [JsonIgnore]
        public Guid UserId { get; set; }

        [JsonIgnore]
        public ApplicationUser? User { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTimeOffset ExpiresAtUtc { get; set; }
        public DateTimeOffset CreatedAtUtc { get; set; }
        public string CreatedByIp { get; set; } = string.Empty;
        public DateTimeOffset? RevokedAtUtc { get; set; }
        public string? RevokedByIp { get; set; }
        public string? ReplacedByToken { get; set; }
        public string? ReasonRevoked { get; set; }
        public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAtUtc;
        public bool IsRevoked => RevokedAtUtc != null;
        public bool IsActive => !IsRevoked && !IsExpired;
    }
}
