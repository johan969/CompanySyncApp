namespace Arb_Int.Domain.User
{
    public class ChangePasswordRequest
    {
        public required string Email { get; set; }
        public required string CurrentPassword { get; set; }

        public required string NewPassword { get; set; }
    }
}