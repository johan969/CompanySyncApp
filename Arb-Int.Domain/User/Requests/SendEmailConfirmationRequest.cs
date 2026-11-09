namespace Arb_Int.Domain.User
{
    public class UserSendEmailConfirmation
    {
        public required string Email { get; set; }
        public required string ClientUrl { get; set; }
    }
}