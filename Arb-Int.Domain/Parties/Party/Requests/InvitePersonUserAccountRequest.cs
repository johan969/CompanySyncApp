namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Innehåller data för att koppla ett befintligt Identity-konto till en person.
    /// </summary>
    public sealed class InvitePersonUserAccountRequest
    {
        /// <summary>Identifierar Identity-användaren som ska kopplas till personen.</summary>
        public Guid ApplicationUserId { get; init; }
        /// <summary>Identifierar den aktiva e-postkontaktpunkt som representerar Identity-kontots auth-adress.</summary>
        public Guid AuthEmailContactPointId { get; init; }
        /// <summary>Anger om användaren är extern.</summary>
        public bool IsExternalUser { get; init; }
        /// <summary>Interna anteckningar om kontokopplingen.</summary>
        public string? Notes { get; init; }
    }
}
