using System.ComponentModel.DataAnnotations;

namespace Arb_Int.Domain.User
{
    public class ForgotPasswordRequest
    {
        public required string Email { get; set; }

        public required string ClientUrl { get; set; }
    }
}