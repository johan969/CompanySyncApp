namespace Arb_Int.Domain.User
{
    public class ResetPasswordRequest
    {
        public required string Password { get; set; }

        public required string Email { get; set; }
        public required string Token { get; set; }
    }
}