using System.Text.Json.Serialization;

namespace Arb_Int.Domain.User
{
    public class AuthJwtResponse
    {
        public string? AccessToken { get; set; }
        public double ExpiresIn { get; set; }
        public UserData? UserData { get; set; }
        public string TokenType { get; set; } = "Bearer";

        public bool IsAuthSuccessful { get; set; }
        public bool IsPasswordChangeRequired { get; set; }
        public string? ErrorMessage { get; set; }
        public bool Is2StepVerificationRequired { get; set; }
        public string? Provider { get; set; }
        public bool IsEmailConfirmationNeeded { get; set; }
        public string? AgreementToSignToken { get; set; }


        [JsonIgnore]
        public string? RefreshToken { get; set; }
    }
}