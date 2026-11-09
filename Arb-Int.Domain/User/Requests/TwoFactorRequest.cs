namespace Arb_Int.Domain.User
{
    public class TwoFactorRequest
    {
        public required string Email { get; set; }
        public required string Provider { get; set; } = "Email";
        public required string Token { get; set; }
    }
}