namespace Arb_Int.Domain.User
{
    public class UserEmailConfirmation
    {
        public required string Email { get; set; }
        public required string Token { get; set; }
    }
}