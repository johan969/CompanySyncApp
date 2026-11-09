namespace Arb_Int.Domain.User
{
    /// <summary>
    /// Innehåller data för att skapa Identity-konto och bjuda in en befintlig organisationsperson.
    /// </summary>
    public sealed class InviteOrganizationUserRequest
    {
        /// <summary>
        /// Identifierar den aktiva e-postkontaktpunkt som invitationen ska skickas till.
        /// </summary>
        public Guid ContactPointId { get; init; }

        /// <summary>
        /// Anger om den skapade PersonUserAccount-kopplingen är extern.
        /// </summary>
        public bool IsExternalUser { get; init; }

        /// <summary>
        /// Frontend-URL som tar emot token och e-post för initial lösenordssättning.
        /// <para>Exempel: https://app.example.com/reset-password</para>
        /// </summary>
        public required string ClientUrl { get; init; }

        /// <summary>
        /// Interna anteckningar om användarkontokopplingen.
        /// </summary>
        public string? Notes { get; init; }
    }
}
