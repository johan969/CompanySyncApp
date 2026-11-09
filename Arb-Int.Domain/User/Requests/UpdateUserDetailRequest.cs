namespace Arb_Int.Domain.User
{
    /// <summary>
    /// Innehåller redigerbara profilfält för en Identity-användare och dess länkade Person.
    /// <para>E-postvärdet ändras inte som fri text här. Auth-e-post byts genom att välja en befintlig aktiv PartyContactPoint.</para>
    /// </summary>
    public sealed class UpdateUserDetailRequest
    {
        /// <summary>Nytt förnamn. För länkade personer synkas värdet till Person.GivenName.</summary>
        public string? FirstName { get; init; }
        /// <summary>Nytt efternamn. För länkade personer synkas värdet till Person.FamilyName.</summary>
        public string? LastName { get; init; }
        /// <summary>Nytt telefonnummer eller null för att rensa Identity-numret.</summary>
        public string? PhoneNumber { get; init; }
        /// <summary>Valfri aktiv e-postkontaktpunkt som ska bli Identity-kontots auth-e-postadress.</summary>
        public Guid? AuthEmailContactPointId { get; init; }
    }
}
