namespace Arb_Int.Domain.User
{
    public class UserData
    {
        public required string Id { get; set; }
        public required string Email { get; set; }
        public required IEnumerable<UserClaim> Claims { get; set; } = [];
    }
}