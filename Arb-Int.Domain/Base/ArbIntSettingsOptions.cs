namespace Arb_Int.Domain.Base
{
    public sealed class ArbIntSettingsOptions
    {
        public const string SectionName = "ArbIntSettings";

        public string AccountName { get; set; } = "Arbiter";

        public string SupportUrl { get; set; } = string.Empty;

        public string SystemOwner { get; set; } = "Algaebra Trading AB";

        public Guid SystemUserId { get; set; }

        public int DefaultLanguageId { get; set; } = 2;

        /// <summary>
        /// Resursnyckel eller fallback-text för systemets e-postsignatur.
        /// </summary>
        public string SystemSignature { get; set; } =
            "SystemEmail.Signature";

        /// <summary>
        /// Resursnyckel eller fallback-text för e-posthälsningen.
        /// </summary>
        public string Greeting { get; set; } =
            "SystemEmail.Greeting";

        public string AllRightsReserved { get; set; } =
            "SystemEmail.AllRightsReserved";

        public string ResetPassword_BodyText { get; set; } =
            "SystemEmail.ResetPassword.BodyText";

        public string ResetPassword_FooterText { get; set; } =
            "SystemEmail.ResetPassword.FooterText";

        public string ResetPassword_ResetText { get; set; } =
            "SystemEmail.ResetPassword.ResetText";

        public string EmailVerification_BodyText { get; set; } =
            "SystemEmail.EmailVerification.BodyText";

        public string EmailVerification_FooterText { get; set; } =
            "SystemEmail.EmailVerification.FooterText";

        public string EmailVerification_VerifyText { get; set; } =
            "SystemEmail.EmailVerification.VerifyText";

        public string TwoStepAuthentication_BodyText { get; set; } =
            "SystemEmail.TwoStepAuthentication.BodyText";

        public string TwoStepAuthentication_FooterText { get; set; } =
            "SystemEmail.TwoStepAuthentication.FooterText";

        public string TwoStepAuthentication_Subject { get; set; } =
            "SystemEmail.TwoStepAuthentication.Subject";

    }
}